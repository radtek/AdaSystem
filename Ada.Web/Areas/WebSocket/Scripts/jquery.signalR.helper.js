﻿function SignalRHelper(serverHub) {
    (function ($) {
        var signalRHelper = {
            Conn: undefined,
            ServerHub: undefined,
            ResultData: undefined,
            PushData: undefined,
            //初始化连接
            InitConn: function () {
                var self = this;
                if (!self.Conn)
                    self.Conn = $.connection;
            },
            //创建服务代理
            Start: function () {
                var self = this;
                self.InitConn();
                if (!serverHub)
                    serverHub = "hello";
                self.ServerHub = self.Conn[serverHub];
                self.Conn.hub.start();
            },
            //订阅服务端推送的数据
            SubscribeServerData: function (clientMethodName, callBack) {
                var self = this;
                self.ResultData = null;
                self.ServerHub.client[clientMethodName] = function (data) {
                    try {
                        self.ResultData = JSON.parse(data);
                    } catch (e) {
                        self.ResultData = data;
                    }
                    callBack(self.ResultData);
                };
            },
            //上送数据到服务端
            PushClientData: function () {
                var self = this;
                if (arguments.length == 2) {
                    self.ServerHub.server[arguments[0]](arguments[1]);
                } else if (arguments.length == 3) {
                    self.ServerHub.server[arguments[0]](arguments[1], arguments[2]);
                } else if (arguments.length == 4) {
                    self.ServerHub.server[arguments[0]](arguments[1], arguments[2], arguments[3]);
                }
            },
        };

        signalRHelper.Start();
        $.SignalRHelper = signalRHelper;

    })(jQuery);
}