using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Business.Helpers;
using Domain.Entitys;
using Infra.DAOS;
using Newtonsoft.Json;

namespace Business.Business
{
    public class ComicBusiness
    {
        public ComicDAO ComicDAO { get; set; }

        public ComicBusiness()
        {
            ComicDAO = new ComicDAO();
        }

        public List<Comic> BuscandoComic(int id_personagem, int limit, int offset, out int total)
        {
            try
            {
                Comic comic;
                List<Comic> lista_comic = new List<Comic>();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    //Define chaves e váriaveis de conexão para API

                    string ts = DateTime.Now.Ticks.ToString();
                    string publicKey = "3dac5671135fab935c77add36e46abb6";
                    string hash = Helper.GetHash(ts, publicKey, "c109c5ae5fe25f81bc149b4cf7a2407678d5dc40");

                    int size;

                    //Realiza a requisição da API
                    HttpResponseMessage response = client.GetAsync("https://gateway.marvel.com:443/v1/public/characters/" + id_personagem + "/comics?ts=" + ts + "&limit=" + limit + "&offset=" + offset + "&apikey=" + publicKey + "&hash=" + hash).Result;

                    string conteudo = response.Content.ReadAsStringAsync().Result;

                    //Interpreta o JSON retornado pela API
                    dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                    //Define parâmetros de controle para construção da lista sendo size o total retornado, sendo size a quantidade necessaria para o loop e total o total de registros no BD da API
                    size = Convert.ToInt32(resultado.data.count);
                    total = Convert.ToInt32(resultado.data.total);

                    //Cria a lista de personagens manipulando o JSON
                    for (int j = 0; j < size; j++)
                    {
                        comic = new Comic();
                        comic.Id_marvel = resultado.data.results[j].id;
                        comic.Titulo = resultado.data.results[j].title;
                        comic.Descricao = resultado.data.results[j].description;
                        comic.Preco = resultado.data.results[j].prices[0].price;
                        comic.Pic_url = resultado.data.results[j].thumbnail.path + "." +
                            resultado.data.results[j].thumbnail.extension;
                        if (resultado.data.results[j].urls.Count > 0)
                            comic.Wiki_url = resultado.data.results[j].urls[0].url;
                        lista_comic.Add(comic);
                    }                   

                }
                return lista_comic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Comic BuscaPorMarvelId(int id_comic_marvel)
        {
            try
            {
                return ComicDAO.BuscaPorMarvelId(id_comic_marvel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Comic RemovePorMarvelID(int id_comic_marvel)
        {
            try
            {
                var comic = BuscaPorMarvelId(id_comic_marvel);
                if(comic != null)
                {
                    return ComicDAO.RemoveById(comic.Id);
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Salvar(int id_comic_marvel)
        {
            Comic comic;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    //Define chaves e váriaveis de conexão para API
                    string ts = DateTime.Now.Ticks.ToString();
                    string publicKey = "3dac5671135fab935c77add36e46abb6";
                    string hash = Helper.GetHash(ts, publicKey, "c109c5ae5fe25f81bc149b4cf7a2407678d5dc40");

                    //Realiza a requisição da API
                    HttpResponseMessage response = client.GetAsync("https://gateway.marvel.com:443/v1/public/comics/" + id_comic_marvel + "?ts=" + ts + "&apikey=" + publicKey + "&hash=" + hash).Result;

                    string conteudo = response.Content.ReadAsStringAsync().Result;

                    //Interpreta o JSON retornado pela API
                    dynamic resultado = JsonConvert.DeserializeObject(conteudo);
                  
                    //Verifica se o personagem já existe no BD local
                    comic = new Comic();
                    comic = ComicDAO.BuscaPorMarvelId(id_comic_marvel);

                    //Caso não exista cria o obj personagem e o salva localmente
                    if (comic == null)
                    {
                        Comic comic_salvar = new Comic();
                        comic_salvar.Id_marvel = resultado.data.results[0].id;
                        comic_salvar.Titulo = resultado.data.results[0].title;
                        comic_salvar.Descricao = resultado.data.results[0].description;
                        comic_salvar.Preco = resultado.data.results[0].prices[0].price;
                        comic_salvar.Pic_url = resultado.data.results[0].thumbnail.path + "." +
                            resultado.data.results[0].thumbnail.extension;
                        comic_salvar.Wiki_url = resultado.data.results[0].urls[0].url;
                        ComicDAO.Save(comic_salvar);

                        return true;
                    }
                    //Caso já exista retorna mensagem informativa
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Comic> RotornaTodosSalvos()
        {
            try
            {
                return ComicDAO.RetornoSalvos();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
