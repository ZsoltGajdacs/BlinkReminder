using System;
using System.Linq;

namespace BlinkReminder.Helpers
{
    /// <summary>
    /// For random integer generaton, with the last n number not being repeated
    /// </summary>
    public class RandIntMem
    {
        private readonly Random random = new Random();
        private readonly int[] lastNums;
        private int counter;

        /// <summary>
        /// Throws out of range exception if the amount is, or less than zero
        /// </summary>
        /// <param name="amountToRemember">Number of times a completely new number is given</param>
        public RandIntMem(int amountToRemember)
        {
            if (amountToRemember <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountToRemember), "Value must be higher than Zero");
            }

            lastNums = new int[amountToRemember];
            counter = 0;
        }

        /// <summary>
        /// Gives back a random int between the given numbers 
        /// with max being INCLUSIVE. 
        /// Throws Index out of range if max - min is less then the amount the object 
        /// has to remember.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int GetRandInt(int min, int max)
        {
            if (max - min < lastNums.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(max), "Max - Min can't be less then the object's int memory");
            }

            int numToReturn;

            do
            {
                numToReturn = random.Next(min, max + 1);
            } while (lastNums.Contains(numToReturn));

            lastNums[counter] = numToReturn;

            counter = counter == lastNums.Length - 1 ? counter = 0 : counter + 1;

            return numToReturn;
        }
    }
}
