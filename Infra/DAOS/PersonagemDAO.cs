using Domain.Entitys;
using Domain.Helper;
using Infra.DAOS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.DAOS
{
    public class PersonagemDAO : CrudDAO<Personagem>
    {
        public Paginator<Personagem> PaginatorAll(Comic pesquisa, int currentPage = 1, int itensPerPage = 30)
        {
            if (itensPerPage <= 0)
            {
                return null;
            }

            List<Personagem> personagemList = new List<Personagem>();
            int countItens = 0;

            var query = DbSet.Select(c => c);
            if (pesquisa.Id > 0)
                query = query.Where(cat => cat.Id == pesquisa.Id);

            personagemList = query
                .OrderByDescending(c => c.Id)
                .Skip((currentPage - 1) * itensPerPage)
                .Take(itensPerPage)
                .ToList();

            countItens = query.Count();

            return new Paginator<Personagem>(personagemList, countItens, currentPage, itensPerPage);
        }

        public List<Personagem> RetornaTodosSalvos()
        {
           return DbSet.ToList();
        }

        public Personagem BuscaPorMarvelId(int id_personagem_marvel)
        {
            return DbSet.FirstOrDefault(p => p.Id_marvel == id_personagem_marvel);
        }
    }
}
