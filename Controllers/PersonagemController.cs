using Business.Business;
using Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheMarvel.Controllers
{
    public class PersonagemController : Controller
    {
        public PersonagemBusiness PersonagemBusiness { get; set; }

        public PersonagemController()
        {
            PersonagemBusiness = new PersonagemBusiness();
        }

        // GET: Personagem
        public ActionResult Index(int limit, int offset)
        {
            try
            {
                int total = 0;
                List<Personagem> lista_personagens = PersonagemBusiness.BuscaPersonagemListar(limit, offset, out total);

                int paginator = total / limit;
                ViewBag.paginator = paginator;
                ViewBag.limit = limit;
                ViewBag.offset = offset;
                ViewBag.total = total;
                return View(lista_personagens);
            }
            catch (Exception e)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }


        }

        public ActionResult ExcluirPersonagem(int id_personagem_marvel)
        {
            try
            {
                Personagem personagem = PersonagemBusiness.RemovePorMarvelId(id_personagem_marvel);


                //Caso não exista retorna uma mensagem informativa
                if (personagem == null)
                {
                    ViewBag.title = "Erro";
                    ViewBag.message = "Personagem não encontrado";
                    return View("Message");
                }
                //Caso exista exclui o personagem retorna uma mensagem informativa
                else
                {
                    ViewBag.title = "Sucesso";
                    ViewBag.message = "Personagem excluído com sucesso";
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

        public ActionResult SalvarPersonagem(int id_personagem_marvel)
        {


            try
            {
                bool salvou = PersonagemBusiness.Salvar(id_personagem_marvel);

                if (salvou)
                {
                    ViewBag.title = "Sucesso";
                    ViewBag.message = "Personagem salvo com sucesso";
                    return View("Message");
                }
                //Caso já exista retorna mensagem informativa
                else
                {
                    ViewBag.title = "Personagem já salvo";
                    ViewBag.message = "Este personagem já existe na base de dados";

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


        public ActionResult Save()
        {
            try
            {
                List<Personagem> lista_personagem = PersonagemBusiness.RetornoTodosSalvos();

                ViewBag.total_lista = lista_personagem.Count;
                return View(lista_personagem);
            }
            catch (Exception e)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }



        }


        public ActionResult Paralelo()
        {
            try
            {
                PersonagemBusiness.BuscaPersonagens(true);
                ViewBag.title = "Personagens salvos";
                ViewBag.message = "Paralelo executado";

                return View("Message");
            }
            catch(Exception ex)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }
            
        }

        public ActionResult Sequencial()
        {
            try
            {
                PersonagemBusiness.BuscaPersonagens(false);
                ViewBag.title = "Personagens salvos";
                ViewBag.message = "Sequencial executado";

                return View("Message");
            }
            catch (Exception ex)
            {
                ViewBag.title = "Um erro ocorreu";
                ViewBag.message = "Um erro ocorreu, por favor, tente novamente";
                return View("Message");
            }
        }
    }
}