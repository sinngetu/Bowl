﻿using Bowl.Common;
using Bowl.Models.Entities;
using Bowl.Models.LocalSocket;
using Bowl.Models.Request;
using Bowl.Services.Business;
using System.Text.Json;

namespace Bowl.Services.LocalSocket
{
    public static class Distributer
    {
        public static Errors Distribute(Information info)
        {
            var (err, data) = Handle(info);
            return Utils.ErrorHandle(err, data);
        }

        static (ErrorType, object? data) Handle(Information info)
        {
            switch (info.Action)
            {
                case Information.ActionType.SetWeiboHotlist: return SetWeiboHotlist(info.Data);
                case Information.ActionType.AddBossNews: return AddBossNews(info.Data);
                case Information.ActionType.BigNews: return BigNews(info.Data);
                default: return (ErrorType.NoError, null);
            }
        }

        // TODO: If there are too many types of actions, handle functions need to be separated into other files

        static (ErrorType, bool) SetWeiboHotlist(JsonElement? data)
        {
            if (!data.HasValue)
                return (ErrorType.InvalidArgument, false);

            try
            {
                using (var scope = Utils.serviceFactory.CreateScope())
                {
                    var raw = data.Value.GetProperty("raw").Deserialize<List<RawWeiboHotlist>>();
                    var hashes = data.Value.GetProperty("hashes").Deserialize<List<string>>();
                    var service = scope.ServiceProvider.GetRequiredService<IHotlistService>();

                    service.SetWeiboList(raw, hashes);
                    return (ErrorType.NoError, true);
                }
            }
            catch (JsonException ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.InvalidArgument, false);
            }
            catch (InvalidOperationException ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.ServiceError, false);
            }
            catch (Exception ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.UnknowError, false);
            }
        }

        static (ErrorType, bool) AddBossNews(JsonElement? data)
        {
            if (!data.HasValue)
                return (ErrorType.InvalidArgument, false);

            try
            {
                using (var scope = Utils.serviceFactory.CreateScope())
                {
                    var bossNews = data.Value.Deserialize<List<Boss>>();
                    var service = scope.ServiceProvider.GetRequiredService<INewsService>();

                    return service.AddBossNews(bossNews);
                }
            }
            catch (JsonException ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.InvalidArgument, false);
            }
            catch (InvalidOperationException ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.ServiceError, false);
            }
            catch (Exception ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.UnknowError, false);
            }
        }

        static (ErrorType, bool) BigNews(JsonElement? data)
        {
            if (!data.HasValue)
                return (ErrorType.InvalidArgument, false);

            try
            {
                using (var scope = Utils.serviceFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    var msg = data.Value.GetRawText();

                    // TODO: port needs to be configured
                    return service.Notify(8151, msg);
                }
            }
            catch (InvalidOperationException ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.ServiceError, false);
            }
            catch (ArgumentNullException ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.InvalidArgument, false);
            }
            catch (NotSupportedException ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.InvalidArgument, false);
            }
            catch (Exception ex)
            {
                Utils.logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.UnknowError, false);
            }
        }
    }
}
