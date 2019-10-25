using Business.Helpers;
using Domain.Entitys;
using Infra.DAOS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class PersonagemBusiness
    {
        public static List<Personagem> Personagens = new List<Personagem>();
        public PersonagemDAO PersonagemDAO { get; set; }

        public PersonagemBusiness()
        {
            PersonagemDAO = new PersonagemDAO();
        }


        public void BuscaPersonagens(bool paralelo=true)
        {
            try
            {
                int total = 0;
                using (var client = new HttpClient())
                {
                    int offset = 0;
                    int limit = 1;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Define chaves e váriaveis de conexão para API
                    string ts = DateTime.Now.Ticks.ToString();
                    string publicKey = "3dac5671135fab935c77add36e46abb6";
                    string hash = Helper.GetHash(ts, publicKey, "c109c5ae5fe25f81bc149b4cf7a2407678d5dc40");

                    int size;

                    //Realiza a requisição da API
                    HttpResponseMessage response = client.GetAsync("https://gateway.marvel.com:443/v1/public/characters?ts=" + ts + "&limit=" + limit + "&offset=" + offset + "&apikey=" + publicKey + "&hash=" + hash).Result;

                    string conteudo = response.Content.ReadAsStringAsync().Result;

                    //Interpreta o JSON retornado pela API
                    dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                    //Define parâmetros de controle para construção da lista sendo size o total retornado, sendo size a quantidade necessaria para o loop e total o total de registros no BD da API
                    size = Convert.ToInt32(resultado.data.count);
                    total = Convert.ToInt32(resultado.data.total);
                }

                if (paralelo)
                {
                    BuscaParalelo(total);
                }
                else
                {
                    BuscaSequencial(total);
                }


            }
            catch (Exception e)
            {

            }
        }

        public bool Salvar(int id_personagem_marvel)
        {
            Personagem personagem;
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
                    HttpResponseMessage response = client.GetAsync("https://gateway.marvel.com:443/v1/public/characters/" + id_personagem_marvel + "?apikey=" + publicKey + "&hash=" + hash + "&ts=" + ts).Result;

                    string conteudo = response.Content.ReadAsStringAsync().Result;

                    //Interpreta o JSON retornado pela API
                    dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                    personagem = PersonagemDAO.BuscaPorMarvelId(id_personagem_marvel);

                    

                    //Caso não exista cria o obj personagem e o salva localmente
                    if (personagem == null)
                    {
                        Personagem personagem_salvar = new Personagem();
                        personagem_salvar.Id_marvel = resultado.data.results[0].id;
                        personagem_salvar.Nome = resultado.data.results[0].name;
                        personagem_salvar.Descricao = resultado.data.results[0].description;
                        personagem_salvar.Pic_url = resultado.data.results[0].thumbnail.path + "." +
                            resultado.data.results[0].thumbnail.extension;
                        personagem_salvar.Wiki_url = resultado.data.results[0].urls[1].url;
                        PersonagemDAO.Save(personagem_salvar);

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

        public Personagem BuscaPorMarvelId(int id_personagem_marvel)
        {
            try
            {
                return PersonagemDAO.BuscaPorMarvelId(id_personagem_marvel);
            }
            catch(Exception ex )
            {
                throw ex;
            }
            
        }

        public Personagem RemovePorMarvelId(int id_personagem_marvel)
        {
            try
            {
                var personagem = BuscaPorMarvelId(id_personagem_marvel);
                return PersonagemDAO.RemoveById(personagem.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Personagem> RetornoTodosSalvos()
        {
            try
            {
                return PersonagemDAO.RetornaTodosSalvos();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public List<Personagem> BuscaPersonagemListar(int limit, int offset,out int total)
        {
            try
            {
                Personagem personagem;
                List<Personagem> listaPersonagens = new List<Personagem>();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Define chaves e váriaveis de conexão para API
                    string ts = DateTime.Now.Ticks.ToString();
                    string publicKey = "3dac5671135fab935c77add36e46abb6";
                    string hash = Helper.GetHash(ts, publicKey, "c109c5ae5fe25f81bc149b4cf7a2407678d5dc40");

                    int size;

                    //Realiza a requisição da API
                    HttpResponseMessage response = client.GetAsync("https://gateway.marvel.com:443/v1/public/characters?ts=" + ts + "&limit=" + limit + "&offset=" + offset + "&apikey=" + publicKey + "&hash=" + hash).Result;

                    string conteudo = response.Content.ReadAsStringAsync().Result;

                    //Interpreta o JSON retornado pela API
                    dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                    //Define parâmetros de controle para construção da lista sendo size o total retornado, sendo size a quantidade necessaria para o loop e total o total de registros no BD da API
                    size = Convert.ToInt32(resultado.data.count);
                    total = Convert.ToInt32(resultado.data.total);

                    //Cria a lista de personagens manipulando o JSON
                    for (int j = 0; j < size; j++)
                    {
                        personagem = new Personagem();
                        personagem.Id_marvel = resultado.data.results[j].id;
                        personagem.Nome = resultado.data.results[j].name;
                        personagem.Descricao = resultado.data.results[j].description;
                        personagem.Pic_url = resultado.data.results[j].thumbnail.path + "." +
                            resultado.data.results[j].thumbnail.extension;
                        if (resultado.data.results[j].urls.Count > 0)
                            personagem.Wiki_url = resultado.data.results[j].urls[0].url;
                        listaPersonagens.Add(personagem);
                    }
                }
                return listaPersonagens;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        Action<int> Busca = (int offset) =>
        {
            try
            {
                Personagem personagem;


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
                    HttpResponseMessage response = client.GetAsync("https://gateway.marvel.com:443/v1/public/characters?ts=" + ts + "&limit=1&offset=" + offset + "&apikey=" + publicKey + "&hash=" + hash).Result;

                    string conteudo = response.Content.ReadAsStringAsync().Result;

                    //Interpreta o JSON retornado pela API
                    dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                    //Cria a lista de personagens manipulando o JSON

                    personagem = new Personagem();
                    personagem.Id_marvel = resultado.data.results[0].id;
                    personagem.Nome = resultado.data.results[0].name;
                    personagem.Descricao = resultado.data.results[0].description;
                    personagem.Pic_url = resultado.data.results[0].thumbnail.path + "." +
                        resultado.data.results[0].thumbnail.extension;
                    if (resultado.data.results[0].urls.Count > 0)
                        personagem.Wiki_url = resultado.data.results[0].urls[0].url;
                    Personagens.Add(personagem);




                }

            }
            catch (Exception e)
            {

            }
        };



        private void BuscaParalelo(int total)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 5;
            Parallel.For(0, total, parallelOptions, Busca);

            PersonagemDAO.SaveBach(Personagens);
            Personagens = new List<Personagem>();

        }

        private void BuscaSequencial(int total)
        {
            for (int offset = 0; offset < total; offset++)
            {
                try
                {
                    Personagem personagem;


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
                        HttpResponseMessage response = client.GetAsync("https://gateway.marvel.com:443/v1/public/characters?ts=" + ts + "&limit=1&offset=" + offset + "&apikey=" + publicKey + "&hash=" + hash).Result;

                        string conteudo = response.Content.ReadAsStringAsync().Result;

                        //Interpreta o JSON retornado pela API
                        dynamic resultado = JsonConvert.DeserializeObject(conteudo);



                        //Cria a lista de personagens manipulando o JSON

                        personagem = new Personagem();
                        personagem.Id_marvel = resultado.data.results[0].id;
                        personagem.Nome = resultado.data.results[0].name;
                        personagem.Descricao = resultado.data.results[0].description;
                        personagem.Pic_url = resultado.data.results[0].thumbnail.path + "." +
                            resultado.data.results[0].thumbnail.extension;
                        if (resultado.data.results[0].urls.Count > 0)
                            personagem.Wiki_url = resultado.data.results[0].urls[0].url;
                        Personagens.Add(personagem);




                    }
                    PersonagemDAO.SaveBach(Personagens);
                    Personagens = new List<Personagem>();

                }
                catch (Exception e)
                {

                }
            }




        }
    }
}
