/*------------------------------------------	
*jquery扩展，加载js文件和css文件
-------------------------------------------*/
/*!
* includFile.js
* Copyright 2013, lee b
* 
* Email:1280003243@qq.com
* Date: 2012-08-01
*/
$.extend({
    includFile: function (includePath, file) {
        var files = typeof file == "string" ? [file] : file;
        for (var i = 0; i < files.length; i++) {
            var name = files[i].replace(/^\s|\s$/g, "");
            var att = name.split('.');
            var ext = att[att.length - 1].toLowerCase();
            var isCss = ext == "css"; var tag = isCss ? "link" : "script"; var attr = isCss ? " type='text/css' rel='stylesheet' " : " type='text/javascript' "; var link = (isCss ? "href" : "src") + "='" + includePath + name + "'"; if ($(tag + "[" + link + "]").length == 0) document.write("<" + tag + attr + link + "></" + tag + ">");
        }
    }
});
//js文件
//$.includFile("/Scripts/", ['plugin/jquery.easing.min.js',  'plugin/jquery.lazyload.js']);



//声明命名空间-- 弹出框需引用 http://layer.layui.com/
if (!window.ART) {
    window.ART = {};
}
if (!window.ART.popBox) {
    window.ART.popBox = {};
}
//提示框
window.ART.popBox.Tips = function (msg, i, time) {
    var shift = [0, 1, 2, 3, 4, 5, 6];
    layer.msg(msg, {
        icon: i,
        shift: shift[Math.floor(Math.random() * shift.length)],
        time: time * 1000 //2秒关闭（如果不配置，默认是3秒）
    });
}
//弹出系统正在处理提示
window.ART.popBox.Doing = function (msg) {
    if (msg == null) {
        msg = "系统正在处理…请稍后！";
    }
    layer.open({
        type: 1,
        title: "提示",
        closeBtn: 0,
        shadeClose: false,
        content: '<p style="padding: 20px;"><img src="//img.ejiadg.cn/assets/global/plugins/layer/skin/default/loading-0.gif" /> ' + msg + '</p>'
    });
}

//选择框
window.ART.popBox.ConfirmFunc = function (msg, func) {
    layer.confirm(msg, { icon: 3, title: '提示', shift: 6 }, function (index) {
        func();
        layer.close(index);
    });
}
//选择框--点击确认并跳转到指定页面
window.ART.popBox.ConfirmGoTo = function (msg, url) {
    layer.confirm(msg, { icon: 3, title: '提示', shift: 6 }, function (index) {
        window.location = url;
    });
}
//确认框
window.ART.popBox.Sure = function (msg) {
    var shift = [0, 1, 2, 3, 4, 5, 6];
    layer.alert(msg, { icon: 1, shift: shift[Math.floor(Math.random() * shift.length)] });
}
//确认框
window.ART.popBox.SureCallback = function (msg, func) {
    var shift = [0, 1, 2, 3, 4, 5, 6];
    layer.alert(msg, { icon: 4, shift: shift[Math.floor(Math.random() * shift.length)] }, function (index) {
        func();
        layer.close(index);
    });
}

//确认框
window.ART.popBox.SureGoTo = function (msg, url) {
    var shift = [0, 1, 2, 3, 4, 5, 6];
    layer.alert(msg, { icon: 4, shift: shift[Math.floor(Math.random() * shift.length)] }, function (index) {
        window.location = url;
        layer.close(index);
    });
}

//ajax get请求数据方式
function st_Get(url, data, func) {
    $.getJSON("/Ajax/" + url + ".ashx", data, func);
}
//ajax post 请求方式
function st_Post(url, data, func) {
    $.post("/Ajax/" + url + ".ashx", data, func, "json");
}

//ajax get请求数据方式
function ejia_Get(url, data, func) {
    $.getJSON(url, data, func);
}
//ajax post 请求方式
function ejia_Post(url, data, func) {
    $.post(url, data, func, "json");
}


//========================基于Validform插件========================
//初始化验证表单
$.fn.initValidform = function () {
    var checkValidform = function (formObj) {
        $(formObj).Validform({
            tiptype: function (msg, o, cssctl) {
                if (!o.obj.is("form")) {
                    //页面上不存在提示信息的标签时，自动创建;
                    if (o.obj.parent().find(".Validform_checktip").length == 0) {
                        o.obj.parent().append("<b class='Validform_checktip' />");

                    } else {
                        o.obj.parent().find("b").remove();
                        o.obj.parent().append("<b class='Validform_checktip' />");
                    }
                    var objtip = o.obj.next(".Validform_checktip");
                    cssctl(objtip, o.type);
                    objtip.text(msg);
                }
            },
            showAllError: true
        });
    };
    return $(this).each(function () {
        checkValidform($(this));
    });
}

//发送验证码
var SendVCode = function (objbtn, mobile) {
    var $sendSmsBtn = objbtn;
    $sendSmsBtn.attr("disabled", !0).val("请稍后...");
    var timeout = 60;
    st_Post("SendSmsCode", {
        phoneNo: mobile,
        time: (new Date).getTime()
    }, function (data) {
        if (data.ret == 1) {
            $sendSmsBtn.val("发送成功");
            var timer = setInterval(function () {
                $sendSmsBtn.val(timeout + "秒后重新发送").addClass("sendSmsBtndis");
                timeout--;
                if (timeout < 0) {
                    clearInterval(timer);
                    $sendSmsBtn.removeAttr("disabled").removeClass("sendSmsBtndis").val("获取验证码");
                }
            }, 1000);
        } else {
            $sendSmsBtn.removeAttr("disabled").removeClass("sendSmsBtndis").val("获取验证码");
            alert(data.msg);
        }
    });
}


//所有页面加载公用方法
$(document).ready(function () {
});