using Mikos.XK.Fiscal.Dtos;
using System.Collections.Generic;

namespace Mikos.XK.Fiscal.Model
{
    public class ItemsPlaceholderObject
    {
        public List<Posting> postings { get; set; }
        public List<RevenueBucketInfo> revenueBucketInfos { get; set; }
        public TotalInfo totalInfo { get; set; }
        public List<TrxInfo> TrxInfo { get; set; }
    }
}
