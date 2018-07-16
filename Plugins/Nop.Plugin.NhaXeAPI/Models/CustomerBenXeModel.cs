using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.NhaXeAPI.Models
{
    public class CustomerBenXeModel
    {
        public CustomerBenXeModel(int _cusId, int _benxeId)
        {
            CustomerId = _cusId;
            BenXeId = _benxeId;
        }
        public int CustomerId { get; set; }
        public int BenXeId { get; set; }
    }
}
