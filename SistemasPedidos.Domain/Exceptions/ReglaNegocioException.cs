using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPedidos.Domain.Exceptions
{
    public class ReglaNegocioException: Exception
    {
        public ReglaNegocioException(string message) : base(message)
        {
        }   
    }
}
