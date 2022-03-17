using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Database
{
    public class ConversationReferenceEntity
    {
        public int Id { get; set; }
        private string _upn;
        public string ActivityId { get; set; }
        public string ChannelId { get; set; }
        public string Locale { get; set; }
        public string ServiceUrl { get; set; }
        public string BotId { get; set; }
        public string UserId { get; set; }
        public string UPN
        {
            get => _upn;
            set => _upn = value.ToLower();
        }
        public string Name { get; set; }
        public string AadObjectId { get; set; }
        public string ConversationId { get; set; }
        //public new string RowKey
        //{
        //    get
        //    {
        //        return base.RowKey;
        //    }
        //    set => base.RowKey = value.ToLower();
        //}
    }
}
