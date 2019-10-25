using Business.Business;
using Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheMarvel.Controllers
{
    public class ComicController : Controller
    {
        ComicBusiness ComicBusiness { get; set; }
        public ComicController()
        {
            ComicBusiness = new ComicBusiness();
        }
        // GET: Comic
        public ActionResult Index(int id_personagem, int limit, int offset)
        {
            try
            {
                int total = 0;
                List<Comic> lista_comic = ComicBusiness.BuscandoComic(id_personagem, limit, offset, out total);


                //Define parâmetros de paginação
                int paginator = total / limit;
                ViewBag.paginator = paginator;
                ViewBag.limit = limit;
                ViewBag.offset = offset;
                ViewBag.id_personagem = id_personagem;
                ViewBag.total = total;
                ViewBag.total_lista = lista_comic.Count;


                return View(lista_comic);
            }
            catch (Exception e)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }
        }

        public ActionResult Save()
        {
            try
            {
                List<Comic> lista_comic = ComicBusiness.RotornaTodosSalvos();
                ViewBag.total_lista = lista_comic.Count;
                return View(lista_comic);
            }
            catch (Exception e)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }
        }

        public ActionResult ExcluirComic(int id_comic_marvel)
        {
            try
            {
                Comic comic = ComicBusiness.RemovePorMarvelID(id_comic_marvel);
                //Caso não exista retorna uma mensagem informativa
                if (comic == null)
                {
                    ViewBag.title = "Erro";
                    ViewBag.message = "Comic não encontrada";
                    return View("Message");
                }
                //Caso exista exclui o personagem retorna uma mensagem informativa
                else
                {                    
                    ViewBag.title = "Sucesso";
                    ViewBag.message = "Comic excluída com sucesso";
                    return View("Message");
                }
            }
            catch (Exception e)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }

        }

        public ActionResult SalvarComic(int id_comic_marvel)
        {
            try
            {
                bool salvou = ComicBusiness.Salvar(id_comic_marvel);

                if (salvou)
                {
                    ViewBag.title = "Sucesso";
                    ViewBag.message = "Comic salva com sucesso";

                    return View("Message");
                }

                else
                {
                    ViewBag.title = "Comic já salva";
                    ViewBag.message = "Esta comic já existe na base de dados";

                    return View("Message");
                }

            }
            catch (Exception e)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }

        }
    }
}