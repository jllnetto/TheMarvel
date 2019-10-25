using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helper
{
    public class Paginator<TEntity>
    {
        /// <summary>
        ///     Atributo IEnumerable com o conteúdo da página resultante da consulta no banco de dados.
        /// </summary>
        public virtual IList<TEntity> Conteudo { get; set; }        

        /// <summary>
        ///     Atributo que representa a quantidade total de itens na tabela do banco de dados em questão.
        /// </summary>
        public int CountItens { get; set; }

        /// <summary>
        ///     Atributo que representa a quantidade de itens por página do paginator.
        /// </summary>
        public int ItensPerPage { get; set; }

        /// <summary>
        ///     Atributo que representa a quantidade de páginas do paginator.
        /// </summary>
        public int CountPages { get; set; }

        /// <summary>
        ///     Atributo que representa a página atual do paginator.
        /// </summary>
        public int PaginaAtual { get; set; }

        /// <summary>
        ///     Atributo que representa a proxima página do paginator.
        /// </summary>
        public int ProximaPagina { get; set; }

        /// <summary>
        ///     Atributo que representa a página anterior do paginator.
        /// </summary>
        public int PaginaAnterior { get; set; }

        /// <summary>
        ///     Atributo que representa o primeiro item da página atual, do paginator.
        /// </summary>
        public int FirstItemOfPage { get; set; }

        /// <summary>
        ///     Atributo que representa o ultimo item da página atual, do paginator.
        /// </summary>
        public int LastItemOfPage { get; set; }

        /// <summary>
        ///     Termo de pesquisa utilizado na listagem.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        ///     Atributo que representa o primeiro da primeira página do paginator.
        /// </summary>
        public TEntity PrimeiroItem()
        {
            return Conteudo.FirstOrDefault();
        }

        /// <summary>
        ///     Construtor Básico.
        /// </summary>
        public Paginator() { }

        /// <summary>
        ///     Construtor avançado - obtem todos os atributos, baseado em:
        ///         Quantidade total de itens.
        ///         Número da página atual.
        ///         Quantidade de itens por página.
        ///         Termo de Pesquisa.
        /// </summary>
        public Paginator(IList<TEntity> conteudo, int countItens, int paginaAtual, int itensPerPage, string searchTerm = null)
        {
            Conteudo = conteudo;
            CountItens = countItens;
            ItensPerPage = itensPerPage;
            SearchTerm = searchTerm;
            PaginaAtual = paginaAtual;
            CountPages = (CountItens / ItensPerPage);
            if (CountItens % ItensPerPage > 0)
            {
                CountPages++;
            }
            ProximaPagina = CountPages > PaginaAtual ? PaginaAtual + 1 : 0;
            PaginaAnterior = PaginaAtual > 1 ? PaginaAtual - 1 : 0;
            FirstItemOfPage = PaginaAnterior * ItensPerPage + 1;
            LastItemOfPage = PaginaAtual * itensPerPage - (PaginaAtual * itensPerPage - CountItens);

            if (CountItens > PaginaAtual * itensPerPage)
            {
                LastItemOfPage = PaginaAtual * itensPerPage;
            }
            else
            {
                LastItemOfPage = CountItens;
            }

        }

    }
}
