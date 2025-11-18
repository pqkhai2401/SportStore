using SportsStoreWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Abstract
{
	public interface IProductRepository
	{
		// Thuộc tính để lấy tất cả sản phẩm
		IQueryable<Product> Products { get; }
		Task SaveProduct(Product product);
	}
}
