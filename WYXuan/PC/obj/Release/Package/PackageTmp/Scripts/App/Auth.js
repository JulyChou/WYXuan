$.includFile("/Scripts/", ['plugin/LayerUi/layui.js', '/plugin/LayerUi/css/layui.css']);
var info = function () {
    var initPage = function () {
        layui.use('form', function () {
            var form = layui.form;
            var layer = layui.layer;
            //自定义验证规则
            form.verify({
                name: function (value) {
                    if (value.length > 10) {
                        return '名字不得超过5个字符啊！';
                    }
                }, Okpass: function (value) {
                    if ($(".registerPwd").val() != value) {
                        return '名两次密码不一致';
                    }
                }
              , pass: [/(.+){6,12}$/, '密码必须6到12位']
            });
            //监听提交
            form.on('submit(submit)', function (data) {
                var $obj = $(this);
                var mode = $obj.data('mode');
                if (mode == "register") {
                    $.post("/Ajax/Register.ashx", $("#registerform").serialize(), function (data) {
                        if (data.status == "y") {
                            ART.popBox.Tips("注册成功，正在跳往登录页面", 1, 2);
                            window.location = "/auth/login";
                            layer.closeAll();
                        } else {
                            layer.closeAll();
                            ART.popBox.Tips(data.info, 2, 2);
                        }
                    });
                }
                if (mode == "login") {
                    $.post("/Ajax/Login.ashx", $("#loginform").serialize(), function (data) {
                        if (data.status == "y") {
                            ART.popBox.Tips("登录成功，正在跳往登录页面", 1, 2);
                            window.location = "/";
                            layer.closeAll();
                        } else {
                            layer.closeAll();
                            ART.popBox.Tips(data.info, 2, 2);
                        }
                    });
                }
                return false;
            });
        });
    }

    return {
        init: function () {
            initPage();
        }
    };
}();

$(function () {
    info.init();
});