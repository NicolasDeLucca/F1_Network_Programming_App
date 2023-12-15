using System;
using System.Collections;
using System.Collections.Generic;

namespace Protocol
{
    public class Log
    {
        public DateTime CreatedAt { get; set; }
        public Command Command { get; set; }
        public dynamic Entity { get; set; }
        public Type EntityType { get; set; }

        public bool IsEntityAList()
        {
            return EntityType.IsGenericType && EntityType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public override string ToString()
        {
            string result;

            if (IsEntityAList())
            {
                IList entityList = Entity as IList;
                result = $"[{CreatedAt}] ({Command}) - \n";

                foreach (var entity in entityList)
                {
                    result += $"{entity} \n";
                }
            }
            else
            {
                result = $"[{CreatedAt}] ({Command}) - {Entity.ToString()}";
            }

            return result;
        }
    }
}
