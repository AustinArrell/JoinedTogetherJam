using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameTEST.Models
{
    public class Deck
    {
        public List<Card> cards { get; set; } = new List<Card>();
        Random rand = new Random();

        public void Shuffle() 
        {
            cards = cards.OrderBy(_ => rand.Next()).ToList();
        }

        public Card DrawCard() 
        {
            if (cards.Count() >= 1)
            {
                Card cardCopy = new Card { Type = cards.ElementAt(0).Type, Owner = cards.ElementAt(0).Owner };
                cards.RemoveAt(0);
                return cardCopy;
            }
            return new Card();
        }
    }
}
