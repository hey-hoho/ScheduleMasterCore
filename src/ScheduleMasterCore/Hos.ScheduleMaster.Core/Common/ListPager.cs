//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;

//namespace Hos.ScheduleMaster.Core.Common
//{
//    /// <summary>
//    /// 列表分页器
//    /// by hoho
//    /// </summary>
//    public class ListPager<TModel>
//    {
//        /// <summary>
//        /// 当前页码
//        /// </summary>
//        //public int PageIndex
//        //{
//        //    get
//        //    {
//        //        int index;
//        //        if (!int.TryParse(System.Web.HttpContext.Current.Request.Params["offset"], out index))
//        //        {
//        //            index = 1;
//        //        }
//        //        return index;
//        //    }
//        //}

//        /// <summary>
//        /// 每页数据条数
//        /// </summary>
//        public int PageSize
//        {
//            get
//            {
//                int size;
//                if (!int.TryParse(System.Web.HttpContext.Current.Request.Params["limit"], out size))
//                {
//                    size = 10;
//                }
//                return size;
//            }
//        }

//        /// <summary>
//        /// 分页要跳过的条数
//        /// </summary>
//        public int SkipCount
//        {
//            get
//            {
//                int offset;
//                if (!int.TryParse(HttpContext.Current.Request.Params["offset"], out offset))
//                {
//                    offset = 0;
//                }
//                return offset;
//            }
//        }

//        /// <summary>
//        /// 总数据量
//        /// </summary>
//        public int Total { get; set; }

//        /// <summary>
//        /// 分页结果
//        /// </summary>
//        public IEnumerable<TModel> Rows;

//        /// <summary>
//        /// 自定义过滤条件
//        /// </summary>
//        internal List<Expression<Func<TModel, bool>>> Filters { get; set; }

//        /// <summary>
//        /// 添加查询条件
//        /// </summary>
//        /// <param name="filter"></param>
//        public void AddFilter(Expression<Func<TModel, bool>> filter)
//        {
//            if (this.Filters == null)
//            {
//                this.Filters = new List<Expression<Func<TModel, bool>>>();
//            }
//            this.Filters.Add(filter);
//        }
//    }
//}
