using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository.IRepository
{
    public interface iConverter <EntityModel , ViewModel>
        where EntityModel : class 
        where ViewModel : class
    {
        ViewModel ConvertToModel(EntityModel self);
    }
}
