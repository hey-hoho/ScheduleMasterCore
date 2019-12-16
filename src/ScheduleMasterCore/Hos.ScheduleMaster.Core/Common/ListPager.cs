using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Hos.ScheduleMaster.Core.Common
{

    /// <summary>
    /// 列表分页器
    /// by hoho
    /// </summary>
    public class ListPager<TModel>
    {
        public ListPager(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 分页要跳过的条数
        /// </summary>
        public int SkipCount
        {
            get
            {
                if (PageIndex < 1)
                {
                    PageIndex = 1;
                }
                if (PageSize < 1)
                {
                    PageSize = 10;
                }
                return (PageIndex - 1) * PageSize;
            }
        }

        /// <summary>
        /// 总数据量
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 分页结果
        /// </summary>
        public IEnumerable<TModel> Rows;

        /// <summary>
        /// 自定义过滤条件
        /// </summary>
        internal List<Expression<Func<TModel, bool>>> Filters { get; set; }

        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(Expression<Func<TModel, bool>> filter)
        {
            if (this.Filters == null)
            {
                this.Filters = new List<Expression<Func<TModel, bool>>>();
            }
            this.Filters.Add(filter);
        }
    }
}
