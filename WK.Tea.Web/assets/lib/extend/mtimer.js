layui.define(['jquery'], function (exports) {

    var $ = layui.jquery;//, layer = layui.mobile.layer;

    var Class = function (options) {
        var that = this;
        that.index = null;
        that.config = $.extend({}, that.config, options);
    }

    Class.prototype.config = {
        dateStart: null,
        dateNum: 15,
        timeStart: 120,
        timeSpace: 30,
        timeNum: 18,
        dateElem: null,
        timeElem: null,
        type: 1
    }

    Class.prototype.init = function () {
        var that = this;
        var containerHTML = '';
        
        if (that.config.type == 1) {
            containerHTML = '<div id="mtimer" class="mt_poppanel">\
                    <div class="mt_panel">\
                        <h3 class="mt_title">预定时间</h3>\
                        <div class="mt_body">\
                            <div class="mt_date">\
                                <ul>\
                                </ul>\
                            </div>\
                            <div class="mt_time">\
                                <ul>\
                                </ul>\
                            </div>\
                        </div>\
                        <div class="mt_confirm">\
                            <a href="javascript:void(0);" class="mt-btn-cancel"></a>\
                        </div>\
                    </div>\
                </div>';
        } else if (that.config.type == 2) {
            containerHTML = '<div id="mtimer" class="mt_poppanel">\
                    <div class="mt_panel">\
                        <h3 class="mt_title">预定时长</h3>\
                        <div class="mt_body">\
                            <div class="mt_timelong">\
                                <ul>\
                                </ul>\
                            </div>\
                        </div>\
                        <div class="mt_confirm">\
                            <a href="javascript:void(0);" class="mt-btn-cancel"></a>\
                        </div>\
                    </div>\
                </div>';
        }

        

        if (that.config.elem) {
            $(that.config.elem).click(function () {
                if(!that.config.dateStart){
                    that.config.dateStart = new Date();
                } 
                that.index = layer.open({
                    type: 1
                    , content: containerHTML
                    , anim: 'up'
                    , style: 'position:fixed; bottom:0; left:0; width: 100%; height: 350px; padding:10px 0; border:none;'
                    , success: function (layero, index) {
                        var container = $(layero).find('.mt_poppanel');

                        if (that.config.type == 1) {
                            that.config.dateElem = $('.mt_date>ul', container),
                            that.config.timeElem = $('.mt_time>ul', container);

                            that.initDate();
                        } else if (that.config.type == 2) {
                            that.config.timeElem = $('.mt_timelong>ul', container);

                            var data = [];
                            if (that.config.initData && typeof that.config.initData == "function") {
                                data = that.config.initData();
                            }
                            that.initTimeLong(data);
                        }

                        //that.events();
                        $('.mt-btn-cancel', container).click(function () {
                            layer.close(that.index);
                        });
                    }
                });
            });
        }
    }

    Class.prototype.initDate = function () {
        var that = this;
        var dateStart = that.config.dateStart,
            sYear = dateStart.getFullYear(),
            sMonth = dateStart.getMonth(),
            sDate = dateStart.getDate();

        for (var i = 0; i < that.config.dateNum; i++) {
            var dateStr = '', nextDate = new Date(sYear, sMonth, sDate + i),
                m = nextDate.getMonth() + 1,
                d = nextDate.getDate(),
                da = nextDate.getDay(),
                w = '日一二三四五六'.charAt(da),
                times = nextDate.getTime();


            if (i == 0) {
                dateStr = '<li class="selected" data-date="' + times + '">今天&nbsp;<span>周' + w + '</span></li>';
            } else if (i == 1) {
                dateStr = '<li data-date="' + times + '">明天&nbsp;<span>周' + w + '</span></li>';
            } else {
                dateStr = '<li data-date="' + times + '">' + m + '月' + d + '日&nbsp;<span>周' + w + '</span></li>';
            }
            var liDate = $(dateStr);

            that.config.dateElem.append(liDate);

            that.dateItemClick(liDate, nextDate);

            if (i == 0) {
                liDate.trigger("click");
            }
        }
    }

    Class.prototype.initTime = function (dateTime, excludeTimes) {
        var that = this;

        var nowTime = new Date(),
            sYear = dateTime.getFullYear(),
            sMonth = dateTime.getMonth(),
            sDate = dateTime.getDate();

        var timeNum = 24 * 60 / that.config.timeSpace;

        that.config.timeElem.empty();
        for (var i = 0; i < timeNum; i++) {
            var timeStr = '',
                totalM = i * that.config.timeSpace,
                h = Math.floor(totalM / 60),
                m = totalM % 60,
                stimes = new Date(sYear, sMonth, sDate, h, m);

            if (stimes < nowTime) {
                continue;
            }

            var currExcludeTimes = excludeTimes[sYear + "/" + (sMonth + 1) + "/" + sDate];
            var isOrdered = currExcludeTimes && currExcludeTimes.indexOf(stimes.format("yyyy-MM-dd hh:mm:ss")) > -1;
            //var isNotEnough = false;
            //判断后续时间是否有2小时预定的充足时间
            if (!isOrdered) {
                var totalTime = stimes.getTime();
                for (var j = 0; j < 4; j++) {
                    var nextTotalTime = totalTime + (j + 1) * that.config.timeSpace * 60 * 1000,
                        nextTime = new Date(nextTotalTime);

                    var nextExcludeTimes = excludeTimes[nextTime.getFullYear() + "/" + (nextTime.getMonth() + 1) + "/" + nextTime.getDate()];

                    isOrdered = nextExcludeTimes && nextExcludeTimes.indexOf(nextTime.format("yyyy-MM-dd hh:mm:ss")) > -1;
                    
                    if (isOrdered) {
                        break;
                    }
                }
            }

            if (!isOrdered) {
            timeStr = '<li' + (isOrdered ? ' class="ordered"' : '') + '>' + stimes.format("hh:mm") + '</li>';
            var liTime = $(timeStr);
            that.config.timeElem.append(liTime);
                that.timeItemClick(liTime, stimes);
            }
        }
    }

    Class.prototype.initTimeLong = function (data) {
        var that = this;

        that.config.timeElem.empty();

        if (data && data.constructor === Array && data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                var timeLong = data[i];

                var timeLongStr = parseFloat(timeLong) / 60 + "小时"
                timeStr = '<li data-time="' + timeLong + '">' + timeLongStr + '</li>';

                var liTimeLong = $(timeStr);
                that.config.timeElem.append(liTimeLong);
                that.timeLongItemClick(liTimeLong, timeLong);
            }
        } else {
            for (var i = 0; i < that.config.timeNum; i++) {
                var timeLong = that.config.timeStart + i * that.config.timeSpace;

                var timeLongStr = parseFloat(timeLong) / 60 + "小时"
                timeStr = '<li data-time="' + timeLong + '">' + timeLongStr + '</li>';

                var liTimeLong = $(timeStr);
                that.config.timeElem.append(liTimeLong);
                that.timeLongItemClick(liTimeLong, timeLong);
            }
        }
        
    }

    Class.prototype.dateItemClick = function (li, time) {
        var that = this;
        li.on("click", function () {
            $(this).parent().find('li.selected').removeClass('selected');
            $(this).addClass('selected');

            var exclude = {};
            if (that.config.dateClick && typeof that.config.dateClick == "function") {
                exclude = that.config.dateClick(time);
            }

            that.initTime(time, exclude);
        })
    }

    Class.prototype.timeItemClick = function (li, time) {
        var that = this;

        li.on("click", function () {
            $(this).parent().find('li.selected').removeClass('selected');
            $(this).addClass('selected');

            that.config.click && typeof that.config.click == "function" && that.config.click(time.format("yyyy-MM-dd hh:mm"));
            $(that.config.elem).find("input[type=text]").val(time.format("MM月dd日 hh:mm"));
            layer.close(that.index);
        })
    }

    Class.prototype.timeLongItemClick = function (li, timeLong) {
        var that = this;

        li.on("click",  function () {
            $(this).parent().find('li.selected').removeClass('selected');
            $(this).addClass('selected');
            that.config.click && typeof that.config.click == "function" && that.config.click(parseInt(timeLong));
            $(that.config.elem).find("input[type=text]").val(parseFloat(timeLong) / 60 + "小时");
            layer.close(that.index);
        })
    }

    Date.prototype.format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1,                 //月份
            "d+": this.getDate(),                    //日
            "h+": this.getHours(),                   //小时
            "m+": this.getMinutes(),                 //分
            "s+": this.getSeconds(),                 //秒
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度
            "S": this.getMilliseconds()             //毫秒
        };

        if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(fmt)) {
                fmt = fmt.replace(
                  RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            }
        }
        return fmt;
    }

    var mtimer = {
        render: function (options) {
            var cls = new Class(options);
            cls.init();
        }
    }
    exports('mtimer', mtimer);
});