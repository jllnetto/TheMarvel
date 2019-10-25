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
    public class ComicDAO : CrudDAO<Comic>
    {

        public Paginator<Comic> PaginatorAll(Comic pesquisa, int currentPage = 1, int itensPerPage = 30)
        {
            if (itensPerPage <= 0)
            {
                return null;
            }

            List<Comic> comicList = new List<Comic>();
            int countItens = 0;

            var query = DbSet.Select(c => c);
            if (pesquisa.Id > 0)
                query = query.Where(cat => cat.Id == pesquisa.Id);

            comicList = query
                .OrderByDescending(c => c.Id)
                .Skip((currentPage - 1) * itensPerPage)
                .Take(itensPerPage)
                .ToList();

            countItens = query.Count();

            return new Paginator<Comic>(comicList, countItens, currentPage, itensPerPage);
        }

        public List<Comic> RetornoSalvos()
        {
           return DbSet.ToList();
        }

        public Comic BuscaPorMarvelId(int id_comic_marvel)
        {
           return DbSet.FirstOrDefault(c => c.Id_marvel == id_comic_marvel);
        }
    }
}
