using Bowl.Common;
using Bowl.Models.Entities;
using Bowl.Models.LocalSocket;
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

        static (ErrorType, bool) SetWeiboHotlist(object data)
        {
            try
            {
                var hashes = (List<string>)data;
                var service = Utils.serviceProvider.GetRequiredService<HotlistService>();

                service.SetWeiboList(hashes);
                return (ErrorType.NoError, true);
            }
            catch (InvalidCastException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.InvalidArgument, false);
            }
            catch (InvalidOperationException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.ServiceError, false);
            }
            catch (Exception ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.UnknowError, false);
            }
        }

        static (ErrorType, bool) AddBossNews(object data)
        {
            try
            {
                var hashes = (List<Boss>)data;
                var service = Utils.serviceProvider.GetRequiredService<NewsService>();

                return service.AddBossNews(hashes);
            }
            catch (InvalidCastException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.InvalidArgument, false);
            }
            catch (InvalidOperationException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.ServiceError, false);
            }
            catch (Exception ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.UnknowError, false);
            }
        }

        static (ErrorType, bool) BigNews(object data)
        {
            try
            {
                var news = (List<News>)data;
                var service = Utils.serviceProvider.GetRequiredService<NotificationService>();
                var msg = JsonSerializer.Serialize(news);

                // TODO: port needs to be configured
                return service.Notify(9999, msg);
            }
            catch (InvalidCastException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.InvalidArgument, false);
            }
            catch (InvalidOperationException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.ServiceError, false);
            }
            catch (ArgumentNullException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.InvalidArgument, false);
            }
            catch (JsonException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.InvalidArgument, false);
            }
            catch (NotSupportedException ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.InvalidArgument, false);
            }
            catch (Exception ex)
            {
                Utils.Log(Utils.logger.LogError, ex);
                return (ErrorType.UnknowError, false);
            }
        }
    }
}
