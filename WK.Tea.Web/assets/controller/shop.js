
layui.define(function (exports) {

    /*
      下面通过 layui.use 分段加载不同的模块，实现不同区域的同时渲染，从而保证视图的快速呈现
    */


    //区块轮播切换
    layui.use(['admin', 'mtimer', 'carousel'], function () {
        var carousel = layui.carousel
            , $ = layui.jquery
        , form = layui.form, mtimer = layui.mtimer
        , admin = layui.admin
        , router = layui.router();

        var nowDate = new Date(),
            nM = nowDate.getMonth() + 1,
            nD = nowDate.getDate(),
            nH = nowDate.getHours(),
            nMin = nowDate.getMinutes();
        if (nMin >= 30) {
            nH++;
            nMin = "00";
        } else {
            nMin = "30";
        }
        if (nH < 10) {
            nH = "0" + nH;
        }
        $('#choiceStartTime  input[type=text]').val(nM + "月" + nD + "日 " + nH + ":" + nMin);

        mtimer.render({
            elem: '#choiceStartTime'
        });

        mtimer.render({
            type: 2,
            elem: '#choiceTimeLong',
            click: function (value) {
                var price = 288;
                var t = value - 120;
                price += t / 30 * 50;

                var timelong = parseFloat(value) / 60;
                $(".price").text(price);
                $(".timelong").text(timelong);
            }
        });


        admin.req({
            url: '/api/shop/' + router.search.id
            , type: 'get'
            , data: {}
            , done: function (res) { //这里要说明一下：done 是只有 response 的 code 正常才会执行。而 succese 则是只要 http 为 200 就会执行
                console.log(res);
                var detailElem = $(['<div class="layui-card-header" style="text-align:center;">'
                    , res.data.ShopName
                    , '</div>'
                    , '<div class="layui-card-body">'
                    , '<div class="detail-info layui-clear">'
                    , '<div style="float:left; width:80%">'
                    , '<i class="layui-icon layui-icon-location"></i>' + res.data.ShopAddress
                    , '</div>'
                    , '<div style="float:right; width:18%; border-left:1px solid #f6f6f6; text-align:center;">'
                    , '<a href="tel:' + res.data.ShopPhoneNum + '"><i class="layui-icon layui-icon-cellphone"></i></a>'
                    , '</div>'
                    , '</div>'
                    , '</div>'
                ].join(''))
                $('.detail-info').append(detailElem);
            }
        });

        //图片轮播
        var ins = carousel.render({
            elem: '#test1'
          , width: '100%'
          , height: '230px'
          , interval: 5000
        });

    });

    exports('shop', {})
});