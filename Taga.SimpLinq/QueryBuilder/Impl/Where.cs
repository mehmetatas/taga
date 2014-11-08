using System;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class Where : IWhere
    {
        public Operator Operator { get; set; }
        public object Operand1 { get; private set; }
        public object Operand2 { get; private set; }

        public void SetOperand(object operand)
        {
            if (Operand1 == null)
            {
                Operand1 = operand;
            }
            else if (Operand2 == null)
            {
                Operand2 = operand;
            }
            else
            {
                throw new Exception("All operands already set!");
            }
        }

        public void Not()
        {
            Operator = ~Operator;

            if (Operand1 is Where)
            {
                ((Where)Operand1).Not();
            }
            if (Operand2 is Where)
            {
                ((Where)Operand2).Not();
            }
        }
    }
}