/**

 @Name：layuiAdmin 主页控制台
 @Author：贤心
 @Site：http://www.layui.com/admin/
 @License：LPPL
    
 */


layui.define(function (exports) {

    /*
      下面通过 layui.use 分段加载不同的模块，实现不同区域的同时渲染，从而保证视图的快速呈现
    */


    layui.use(['admin', 'form'], function () {
        var $ = layui.$
        , admin = layui.admin
        , element = layui.element
        , form = layui.form
        , device = layui.device();


        var getCountOption = function (type, desktopid, callback) {
            admin.req({
                url: '/api/desktop/report/count'
                , type: 'get'
                , data: {
                    type: type,
                    desktopid: desktopid
                }
                , done: function (res) { //这里要说明一下：done 是只有 response 的 code 正常才会执行。而 succese 则是只要 http 为 200 就会执行

                    callback && typeof callback == "function" && callback(res.data);
                }, success: function (res) {

                }
            });
        }, renderDataView = function (elem, data) {
            elem.find("cite")[0].innerHTML = data.UserCount;
            elem.find("cite")[1].innerHTML = data.ComputerCount;

            var progressFilter = elem.find('.layui-progress').attr("lay-filter");
            var percent = 0;
            if (data.ComputerCount > 0) {
                percent = Math.round(parseFloat(data.UserCount) / data.ComputerCount * 100);
            }
            elem.find(".layui-progress-bar").attr("lay-percent", percent + '%');

            if (elem.attr("id") == "properReport") {
                if (percent > 70) {
                    !elem.find(".layui-progress-bar").hasClass("layui-bg-orange") && elem.find(".layui-progress-bar").addClass("layui-bg-orange");
                } else {
                    elem.find(".layui-progress-bar").hasClass("layui-bg-orange") && elem.find(".layui-progress-bar").removeClass("layui-bg-orange");
                }
            }
            element.render('progress');
        };

        getCountOption("", "", function (data) {
            renderDataView($('#allReport'), data);
        })
        getCountOption(1, "", function (data) {
            renderDataView($('#nonProperReport'), data);
        })
        getCountOption(2, "", function (data) {
            renderDataView($('#properReport'), data);
        })

        form.on('select(nonProperDesktop)', function (data) {
            
            getCountOption(1, data.value, function (data) {
                renderDataView($('#nonProperReport'), data);
            })
        });

        form.on('select(properDesktop)', function (data) {
            getCountOption(2, data.value, function (data) {
                renderDataView($('#properReport'), data);
            })
        });

        form.on('select(allDesktop)', function (data) {
            var val = data.value;
            getCountOption(val, "", function (data) {
                if (val == "5") {
                    $($('#allReport').find("li")[1]).hide();
                    $('#allReport').find(".layui-progress").css("visibility", "hidden");
                } else {
                    $($('#allReport').find("li")[1]).show();
                    $('#allReport').find(".layui-progress").css("visibility", "visible");
                }

                renderDataView($('#allReport'), data);
            })
        });
    });

    //数据概览
    layui.use(['admin', 'form', 'echarts'], function () {
        var $ = layui.$
        , admin = layui.admin
        , form = layui.form
        , echarts = layui.echarts;

        form.render();

        var echartsApp = {}, option
      , elemDataView = $('#LAY-index-dataview')
      , renderDataView = function (elem, option) {
          var elemid = elem.attr("id");

          echartsApp[elemid] = echarts.init(elem[0], layui.echartsTheme);

          echartsApp[elemid].setOption(option);
          //window.onresize = echartsApp[elem.attr("id")].resize;
          admin.resize(function () {
              echartsApp[elemid].resize();
          });
      };
        //没找到DOM，终止执行
        if (!elemDataView[0]) return;

        var getSesstionOption = function (type, desktopid, callback) {
            admin.req({
                url: '/api/desktop/report/session'
                , type: 'get'
                , data: {
                    type: type,
                    desktopid: desktopid
                }
                , done: function (res) { //这里要说明一下：done 是只有 response 的 code 正常才会执行。而 succese 则是只要 http 为 200 就会执行
                    var xAxisData = [], seriesData = {
                        ConnectedSessionCount: {
                            name: "最大已连接会话数",
                            data: []
                        }, DisconnectedSessionCount: {
                            name: "最大已断开会话数",
                            data: []
                        }, ConcurrentSessionCount: {
                            name: "最大并发会话数",
                            data: []
                        }
                    };
                    layui.each(res.data, function (index, item) {
                        xAxisData.push(item.Time2);
                        for (var key in seriesData) {
                            seriesData[key].data.push(item[key]);
                        }
                    });
                    option = {
                        title: null,
                        tooltip: {
                            trigger: 'axis'
                        },
                        legend: {
                            data: [
                                seriesData.ConcurrentSessionCount.name,
                                seriesData.ConnectedSessionCount.name
                            ]
                        },
                        xAxis: [{
                            type: 'category',
                            boundaryGap: false,
                            data: xAxisData,
                            axisLine: {
                                lineStyle: {
                                    color: '#777',
                                    width: 1,
                                    type: 'solid'
                                }
                            }
                        }],
                        yAxis: [{
                            type: 'value',
                            axisLine: {
                                lineStyle: {
                                    color: '#777',
                                    width: 1,
                                    type: 'solid'
                                }
                            }
                        }],
                        series: [{
                            name: seriesData.ConcurrentSessionCount.name,
                            type: 'line',
                            smooth: true,
                            //itemStyle: { normal: { areaStyle: { type: 'default' } } },
                            data: seriesData.ConcurrentSessionCount.data
                        }, {
                            name: seriesData.ConnectedSessionCount.name,
                            type: 'line',
                            smooth: true,
                            //itemStyle: { normal: { areaStyle: { type: 'default' } } },
                            data: seriesData.ConnectedSessionCount.data
                        }]
                    };
                    callback && typeof callback == "function" && callback();
                }, success: function (res) {

                }
            });
        }

        getSesstionOption(1, "", function () {
            renderDataView(elemDataView, option);
        });

        form.on('submit(submitSessionReport)', function (data) {
            getSesstionOption(data.field.type, data.field.desktopid, function () {
                renderDataView(elemDataView, option);
            });
        });

        //监听数据概览轮播

        //监听侧边伸缩
        layui.admin.on('side', function () {
            setTimeout(function () {
                renderDataView(elemDataView, option);
            }, 300);
        });

        //监听路由
        layui.admin.on('hash(tab)', function () {
            layui.router().path.join('') || renderDataView(elemDataView, option);
        });
    });


    exports('console', {})
});