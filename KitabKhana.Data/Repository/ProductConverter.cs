using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository
{
    public class ProductConverter : iConverter<Product, ProductViewModel>
    {
        public ProductViewModel ConvertToModel(Product self)
        {
            try
            {
                ProductViewModel model = new ProductViewModel()
                {
                 Id = self.Id,
                 Title = self.Title,
                 Author = self.Author,
                 ListPrice = self.ListPrice,
                 Price100 = self.Price100,
                 ImageUrl = self?.ImageUrl,
                };

                return model;   
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
            throw new NotImplementedException();

        }
    }
}
