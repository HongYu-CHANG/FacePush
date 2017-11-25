using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace SMarto
{
    public class MotorControl
    {
        private int initialDegree;
        private int nowDegree;
        private int maxDegree;
        private bool isClockwise;//順時針是指從0開始

        public void initial(int initialDegree = 0, int maxDegree = 75, bool isClockwise = true)
        {
            this.initialDegree = initialDegree;
            this.nowDegree = initialDegree;
            this.maxDegree = maxDegree;
            this.isClockwise = isClockwise;
        }

        public string getnowDegree()
        {
            return nowDegree.ToString();
        }

        public string reset()
        {
            return initialDegree.ToString();
        }

        public string addDegree(int addDegree = 5)
        {
            if(isClockwise)
                return preventTooBigDegree((nowDegree + addDegree)).ToString();
            else
                return preventTooBigDegree((nowDegree - addDegree)).ToString();
        }

        public string subDegree(int subDegree = 5)
        {
            return addDegree(subDegree * (-1));
        }

        public string moveTo(int goalDegree = 75)
        {
            return preventTooBigDegree(goalDegree).ToString();
        }

        public string repeatMotor(int[] Repeatdegree, int index)
        {
            return preventTooBigDegree(Repeatdegree[index]).ToString();
        }

        private int preventTooBigDegree(int judgmentDegree)
        {
            if (isClockwise)
            {
                if (judgmentDegree <= maxDegree)
                {
                    nowDegree = judgmentDegree;
                    return judgmentDegree;
                }
                else
                {
                    nowDegree = maxDegree;
                    return maxDegree;
                }
            }
            else
            {
                if (judgmentDegree >= maxDegree)
                {
                    nowDegree = judgmentDegree;
                    return judgmentDegree;
                }
                else
                {
                    nowDegree = maxDegree;
                    return maxDegree;
                }
            }
        }
    }
}
