using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();   
            var category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };
            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon,
            };
            _context.Add(pokemonCategory);

            _context.Add(pokemon);

            return Save();
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            _context.Update(pokemon);
            return Save();
        }

        Pokemon IPokemonRepository.GetPokemon(int id)
        {
            return _context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        Pokemon IPokemonRepository.GetPokemon(string name)
        {
            return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        decimal IPokemonRepository.GetPokemonRating(int pokeId)
        {
            var reviews = _context.Reviews.Where(r => r.Pokemon.Id == pokeId);

            if (reviews.Count() <= 0)
                return 0;
            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
        }

        bool IPokemonRepository.PokemonExists(int pokeId)
        {
            return _context.Pokemon.Any(p => p.Id == pokeId);
        }
    }
}
