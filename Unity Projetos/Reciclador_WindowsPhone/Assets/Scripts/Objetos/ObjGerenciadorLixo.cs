using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ObjGerenciadorLixo : MonoBehaviour
{
	static public ObjGerenciadorLixo instancia = null;

	public ObjAreaReciclavel [] areasReciclagem;

	public GameObject []	objLixos;
	public GameObject [] 	objReciclaveis;
	public GameObject		objXPPositivo;
	public GameObject		objXPNegativo;

	public GameObject []	objEasterEggs;

	public GameObject		objDanoJogador;

	public float	tempoJuntarLixos		= 60;
	public float	tempoCriarLixos			= 5;
	public float	demoraPraJuntarLixos	= 5;
	public int 		quantidadeMaximaLixos	= 5;

	public float 	distanciaMinimaEntreLixos	= 80;

	List<ObjLixoMisturado> 
		listaLixos = new List<ObjLixoMisturado>();

	List<ObjReciclavel>
		listaReciclaveis = new List<ObjReciclavel>();

	ObjLixoMisturado lixoPuxando;
	ObjLixoMisturado lixoSendoPuxado;

	float	proximoTempoCriarLixo		= 0;
	float	proximoTempoJuntarLixo		= 0;
	float	proximoTempoFinalizarJuncao	= 0;

	bool 	podeCriarLixo			= true;
	bool 	podeJuntarLixo			= true;
	bool	juntandoLixos			= false;

	Vector2	velocidadeJuncaoLixos	= Vector2.zero;

	Rect	area;

	string 	nomeArquivoCenario		= "Cenario";

	public void Salvar()
	{
		string divisor = "|";
		string saida = "Lixos";

		saida += divisor + listaLixos.Count;

		foreach(ObjLixoMisturado lixo in listaLixos)
		{
			saida += divisor + lixo.Nivel();
		}

		saida += divisor + "Reciclaveis";

		saida += divisor + listaReciclaveis.Count;
		
		foreach(ObjReciclavel reciclavel in listaReciclaveis)
		{
			if (reciclavel.tipo != Reciclavel.Tipo.EasterEgg)
			{
				saida += divisor + ((int)reciclavel.tipo);
			}
			else
			{
				int nivel = (reciclavel.easteregg + 1) * -1;
				saida += divisor + nivel;
				Debug.Log("Salvou EasterEgg "+reciclavel.easteregg + ", Alterado " + nivel);
			}
		}

		PlayerPrefs.SetString(nomeArquivoCenario, saida);
		Debug.Log ("Saida "+saida);
	}

	static public void CarregarEstatico()
	{
		if (instancia)
			instancia.Carregar();
	}

	void Carregar()
	{
		if (PlayerPrefs.HasKey(nomeArquivoCenario) == false)
			return;

		LimparCenario();

		string [] divisor = {"|"};
		string entrada = PlayerPrefs.GetString(nomeArquivoCenario);
		string [] lista = entrada.Split(divisor, System.StringSplitOptions.None);

		Debug.Log ("Entrada "+entrada);

		int indiceGeral = 1;
		int qtd = int.Parse(lista[indiceGeral]);

		indiceGeral++;

		// JEF AQUI EASTEREGG AJEITAR
		for (int i = 0; i < qtd; i++)
		{
			int nivel = int.Parse(lista[indiceGeral]);
			indiceGeral++;

			if (nivel >= 0)
			{
				CriarLixo(nivel, true);
			}
			else
			{
				Debug.Log("Carregou easter "+ (nivel - 1)+", Alterado "+nivel);
				nivel = -nivel;
				CriarReciclavelEasterEgg(nivel - 1);
			}
		}

		indiceGeral ++;
		qtd = int.Parse(lista[indiceGeral]);
		
		indiceGeral++;
		
		for (int i = 0; i < qtd; i++)
		{
			int tipo = int.Parse(lista[indiceGeral]);
			indiceGeral++;
			CriarReciclavel(tipo);
		}

	}

	void Awake()
	{
		area			= GetComponent<RectTransform>().rect;
		//Debug.Log ("Area "+area);

		instancia		= this;

		podeCriarLixo	= true;
		podeJuntarLixo	= true;
		juntandoLixos	= false;

		ProximoTempoLixo();

		proximoTempoJuntarLixo =
			Time.time  + tempoJuntarLixos;

		if (PlayerPrefs.HasKey(nomeArquivoCenario))
		{
			Carregar();
		}
		else
		{
			if (!Jogador.tutorialRodando)
			{
				CriarLixo();
				CriarLixo();
			}
		}
	}

	void ProximoTempoLixo()
	{
		proximoTempoCriarLixo = 
			Time.time + ObjEmpreendimentos.
				TempoCriarLixo(tempoCriarLixos);
	}

	void Update()
	{
		if (Application.loadedLevelName == "Jogo")
		{
			VerificarAproximarLixos();
			VerificarTempoJuntarLixo();
			VerificarTempoCriarLixo();
		}
		else if (Application.loadedLevelName == "Tutorial" && Tutorial.juntando)
		{
			VerificarAproximarLixos();
			VerificarTempoJuntarLixo();
		}
	}

	void VerificarAproximarLixos()
	{
		if (juntandoLixos)
		{
			//*
			if (lixoPuxando == null || lixoSendoPuxado == null)
			{
				juntandoLixos = false;
				return;
			}

			velocidadeJuncaoLixos = 
				(lixoPuxando.transform.position -
					lixoSendoPuxado.transform.position) / 2;
			//*/

			lixoSendoPuxado.transform.Translate(
				velocidadeJuncaoLixos * Time.deltaTime);

			lixoPuxando.transform.Translate(
				-velocidadeJuncaoLixos * Time.deltaTime);

			if (Time.time > proximoTempoFinalizarJuncao)
			{
				FinalizarJuncao();
			}
		}
	}

	void VerificarTempoCriarLixo()
	{
		if (Time.time > proximoTempoCriarLixo)
		{
			if (podeCriarLixo)
			{
				CriarLixo();
			}

			AtribuirProximoTempoCriar();
		}
	}

	void VerificarTempoJuntarLixo()
	{
		if (!juntandoLixos &&
		    Time.time > proximoTempoJuntarLixo)
		{
			if (podeJuntarLixo)
			{
				JuntarLixo();
			}

			AtribuirProximoTempoJuntar();
		}
	}

	void AtribuirProximoTempoJuntar()
	{
		proximoTempoJuntarLixo =
			Time.time + tempoJuntarLixos;
	}

	void AtribuirProximoTempoCriar()
	{
		ProximoTempoLixo();
	}

	public static int ultimoNivelCriadoLixo = 1;
	static Vector2 posicaoNovoLixo = Vector2.zero;
	static public void CriarLixoEstatico(int nivel, Vector2 pos, bool aleatorio = false)
	{
		posicaoNovoLixo = pos;

		if (!aleatorio)
		{
			instancia.CriarLixo(nivel);
		}

		float x = Random.Range(0f,1f) * instancia.area.width + instancia.area.x;
		float y = Random.Range(0f,1f) * instancia.area.height + instancia.area.y;
		
		posicaoNovoLixo = new Vector2(x,y);

		if (aleatorio)
		{
			instancia.CriarLixo(nivel);
		}
	}

	static public void AtribuirProximaJuncao(float tempo)
	{
		instancia.proximoTempoJuntarLixo = tempo + Time.time;
		instancia.podeJuntarLixo = true;
	}

	void CriarLixo(int novoNivel = -1, bool aleatorioForcado = false)
	{
		if (listaLixos.Count >= quantidadeMaximaLixos)
		{
			Debug.Log (Dados.MensagemCenarioCheio(
				quantidadeMaximaLixos));

			UI_Mensanges.AdicionarMensagem(
				Dados.MensagemCenarioCheio(quantidadeMaximaLixos));

			CriarXP(-Jogador.nivel * 2, transform, false);

			return;
		}

		//TODO: Jef, arruma isso
		int min = Jogador.nivel / Jogador.divisorNiveisNivelMinimoMonstrosAparecem;
		int max = Jogador.nivel / Jogador.divisorNiveisNivelMaximoMonstrosAparecem;
		int nivel = Random.Range(min, max);
		if (nivel < 0)
		{
			nivel = 0;
		}
		Debug.Log("Lixo criado, nível: "+nivel);

		float x = Random.Range(0f,1f) * area.width + area.x;
		float y = Random.Range(0f,1f) * area.height + area.y;

		Vector2 pos = new Vector2(x,y);

		if (novoNivel > 0)
		{
			pos = posicaoNovoLixo;
			nivel = novoNivel - 1;
		}

		if (aleatorioForcado)
		{
			pos = new Vector2(x,y);
		}

		if (nivel >= objLixos.Length)
		{
			nivel = objLixos.Length - 1;
		}

		ultimoNivelCriadoLixo = nivel + 1;

		int indice = nivel % 10;
		int indiceCor = ((nivel - 1) / 10) % Jogador.coresMonstros.Length;

		GameObject novoLixo		= Instantiate<GameObject>(objLixos[indice]);
		novoLixo.GetComponent<Image>().color = Jogador.coresMonstros[indiceCor];

		novoLixo.transform.SetParent(transform, false);
		
		ObjLixoMisturado lixo	=
			novoLixo.GetComponent<ObjLixoMisturado>();

		lixo.transform.position = pos;

		bool 	tentarDenovo	= true;
		int		contador		= 1000;
		while (tentarDenovo && contador > 0)
		{
			tentarDenovo = false;
			foreach(ObjLixoMisturado olm in listaLixos)
			{
				if (olm != lixo)
				{
					float distancia = Vector2.Distance(
						lixo.transform.localPosition,
						olm.transform.localPosition);
					if (distancia < distanciaMinimaEntreLixos)
					{
						x = Random.Range(0f,1f) * area.width + area.x;
						y = Random.Range(0f,1f) * area.height + area.y;
						
						lixo.transform.localPosition = new Vector2(x,y);
						tentarDenovo = true;
						contador--;
					}
				}
			}
		}

		ManterNaArea(
			lixo.transform, 
			lixo.GetComponent<RectTransform>().sizeDelta);

		if (novoNivel < 0)
			Som.Tocar(Som.Tipo.AparecerMonstro);

		//Adicionar(novoLixo.GetComponent<ObjLixoMisturado>());
	}

	static public void JuntarEstatico()
	{
		instancia.JuntarLixo();
	}

	void JuntarLixo()
	{
		PararJuncao();
		if (listaLixos.Count > 1)
		{
			//Debug.Log("Os baguio é doido");
			// -1: aleatorio; 0: menor; 1: maior;
			int tipoDeBusca = -1;
			if (Random.value < 0.05f)
			{
				tipoDeBusca = 0;
			}
			else if (tipoDeBusca < 0.75f)
			{
				tipoDeBusca = 1;
			}

			if (tipoDeBusca == 0)
			{
				lixoPuxando = PegarMenorLixo();
				foreach(ObjLixoMisturado olm in listaLixos)
				{
					if (olm.PodeJuntar(lixoPuxando) &&
					    olm.Nivel() <= lixoPuxando.Nivel())
					{
						lixoSendoPuxado = olm;
						break;
					}
				}
				if (lixoSendoPuxado == null)
				{
					int i = (listaLixos.IndexOf(lixoPuxando) + 1)
						% listaLixos.Count;
					lixoSendoPuxado = listaLixos[i];
				}

				//Debug.Log("Juntar menores");
			}
			else if (tipoDeBusca == 1)
			{
				lixoPuxando = PegarMaiorLixo();
				foreach(ObjLixoMisturado olm in listaLixos)
				{
					if (olm.PodeJuntar(lixoPuxando) &&
					    olm.Nivel() >= lixoPuxando.Nivel())
					{
						lixoSendoPuxado = olm;
						break;
					}
				}
				if (lixoSendoPuxado == null)
				{
					int i = (listaLixos.IndexOf(lixoPuxando) + 1)
						% listaLixos.Count;
					lixoSendoPuxado = listaLixos[i];
				}
				//Debug.Log("Juntar maiores");
			}
			else
			{
				int i = Random.Range (0, listaLixos.Count);
				lixoPuxando = listaLixos[i];
				lixoSendoPuxado = lixoPuxando;
				int contador = 1000;
				while (lixoPuxando == lixoSendoPuxado && contador > 0)
				{
					i = Random.Range (0, listaLixos.Count);
					lixoSendoPuxado = listaLixos[i];
					contador--;
				}
				if (contador <= 0)
				{
					i = (listaLixos.IndexOf(lixoPuxando) + 1)
						% listaLixos.Count;
					lixoSendoPuxado = listaLixos[i];
				}
				//Debug.Log("Juntar aleatórios");
			}

			if (lixoSendoPuxado != null)
			{
				juntandoLixos	= true;
				podeJuntarLixo	= false;

				lixoPuxando.BrilharJuntar();
				lixoSendoPuxado.BrilharJuntar();

				velocidadeJuncaoLixos = 
					lixoPuxando.transform.position -
						lixoSendoPuxado.transform.position;

				velocidadeJuncaoLixos /= demoraPraJuntarLixos;

				proximoTempoFinalizarJuncao =
					Time.time + demoraPraJuntarLixos;
			}
		}
	}

	ObjLixoMisturado PegarMenorLixo()
	{
		ObjLixoMisturado menorLixo = null;

		if (listaLixos.Count > 0)
		{
			menorLixo = listaLixos[0];
			foreach(ObjLixoMisturado olm in listaLixos)
			{
				if (olm.Nivel() <= menorLixo.Nivel())
				{
					menorLixo = olm;
				}
			}
		}

		/*
		if (menorLixo.EstaNivelMaximo())
		{
			return null;
		}
		*/

		return menorLixo;
	}

	ObjLixoMisturado PegarMaiorLixo()
	{
		ObjLixoMisturado maiorLixo = null;
		
		if (listaLixos.Count > 0)
		{
			maiorLixo = listaLixos[0];
			foreach(ObjLixoMisturado olm in listaLixos)
			{
				if (olm.Nivel() >= maiorLixo.Nivel())
				{
					maiorLixo = olm;
				}
			}
		}

		/*
		if (maiorLixo.EstaNivelMaximo())
		{
			return null;
		}
		*/
		
		return maiorLixo;
	}

	void PararJuncao()
	{
		if (lixoPuxando != null)
			lixoPuxando.PararBrilhoJuntar();

		if (lixoSendoPuxado != null)
			lixoSendoPuxado.PararBrilhoJuntar();

		lixoPuxando 	= null;
		lixoSendoPuxado	= null;
		juntandoLixos	= false;
		podeJuntarLixo	= true;

		AtribuirProximoTempoJuntar();
	}

	static public bool NoMeioDeUmaJuncao(ObjLixoMisturado lixo)
	{
		if (lixo == instancia.lixoPuxando ||
		    lixo == instancia.lixoSendoPuxado)
		{
			return true;
		}
		return false;
	}

	void FinalizarJuncao()
	{
		//Debug.Log("Juntou!");
		int pontos = lixoPuxando.Nivel() + lixoSendoPuxado.Nivel();
		CriarXP(-pontos, lixoPuxando.transform, false);

		Som.Tocar(Som.Tipo.JuntarMonstro);

		lixoPuxando.Juntar(lixoSendoPuxado);
		PararJuncao();

		Tutorial.juntando = false;
	}

	static public bool Adicionar(ObjLixoMisturado lixo)
	{
		if (instancia == null) return false;

		lock(instancia.listaLixos)
		{
			try 
			{
				if (!instancia.listaLixos.Contains(lixo))
				{
					lixo.transform.SetParent(
						instancia.transform, false);

					instancia.listaLixos.Add(lixo);
				}
				return true;
			}
			catch (UnityException e)
			{
				Debug.Log (e.Message);
			}
		}
		return false;
	}

	// TODO: JEF
	static public void LimparMonstros()
	{
		if (!instancia) return;
		foreach(ObjLixoMisturado lixo in instancia.listaLixos)
		{
			Destroy(lixo.gameObject);
		}
		instancia.listaLixos.Clear();
	}

	static public void LimparReciclaveis()
	{
		if (!instancia) return;
		foreach(ObjReciclavel reci in instancia.listaReciclaveis)
		{
			Destroy(reci.gameObject);
		}
		instancia.listaReciclaveis.Clear();
	}

	static public void LimparCenario()
	{
		LimparMonstros();
		LimparReciclaveis();
	}

	static public bool Remover(ObjLixoMisturado lixo)
	{
		if (instancia == null) return false;

		lock(instancia.listaLixos)
		{
			try 
			{
				if (instancia.listaLixos.Contains(lixo))
				{
					if (lixo == instancia.lixoPuxando ||
					    lixo == instancia.lixoSendoPuxado)
					{
						instancia.PararJuncao();
					}
					instancia.listaLixos.Remove(lixo);
				}
				return true;
			}
			catch (UnityException e)
			{
				Debug.Log (e.Message);
			}
		}
		return false;
	}

	static public bool Adicionar(ObjReciclavel reciclavel)
	{
		if (instancia == null) return false;
		
		lock(instancia.listaReciclaveis)
		{
			try 
			{
				if (!instancia.listaReciclaveis.Contains(reciclavel))
				{
					reciclavel.transform.SetParent(
						instancia.transform, false);
					
					instancia.listaReciclaveis.Add(reciclavel);
				}
				return true;
			}
			catch (UnityException e)
			{
				Debug.Log (e.Message);
			}
		}
		return false;
	}
	
	static public bool Remover(ObjReciclavel reciclavel)
	{
		if (instancia == null) return false;
		
		lock(instancia.listaReciclaveis)
		{
			try 
			{
				if (instancia.listaReciclaveis.Contains(reciclavel))
				{
					instancia.listaReciclaveis.Remove(reciclavel);
				}
				return true;
			}
			catch (UnityException e)
			{
				Debug.Log (e.Message);
			}
		}
		return false;
	}

	static public void ManterNaArea(Transform lixo, Vector2 tam)
	{
		Rect area = instancia.area;
		
		if (lixo.localPosition.x < area.x + tam.x / 2)
		{
			lixo.localPosition = new Vector3(
				area.x + tam.x / 2,
				lixo.localPosition.y,
				lixo.localPosition.z);
		}
		
		if (lixo.localPosition.x > area.x + area.width - tam.x / 2)
		{
			lixo.localPosition = new Vector3(
				area.x + area.width - tam.x / 2,
				lixo.localPosition.y,
				lixo.localPosition.z);
		}
		
		
		if (lixo.localPosition.y < area.y + tam.y / 2)
		{
			lixo.localPosition = new Vector3(
				lixo.localPosition.x,
				area.y + tam.y / 2,
				lixo.localPosition.z);
		}
		
		if (lixo.localPosition.y > area.y + area.height - tam.y / 2)
		{
			lixo.localPosition = new Vector3(
				lixo.localPosition.x,
				area.y + area.height - tam.y / 2,
				lixo.localPosition.z);
		}
	}

	static public void VerificarSoltarReciclavel(
		ObjReciclavel reciclavel, Vector2 area)
	{

		foreach(ObjAreaReciclavel oar in instancia.areasReciclagem)
		{
			if (oar.tipo == reciclavel.tipo)
			{
				Vector2 p 	= reciclavel.transform.position;
				Vector2 o	= oar.transform.position;
				Vector2 a	= (oar.area / 2) * 
					FuncoesCamera.relacaoAltura;

				if (p.x >= o.x - a.x &&
				    p.x <= o.x + a.x &&
				    p.y >= o.y - a.y &&
				    p.y <= o.y + a.y)
				{
					if (!oar.Reciclar(reciclavel))
					{
						ManterNaArea(reciclavel.transform, area);
						Som.Tocar(Som.Tipo.ErrarLixeira);

						/*
						if (Jogador.tutorialRodando && Tutorial.esperarTentarJogarNaLixeira)
						{
							Tutorial.esperarTentarJogarNaLixeira = false;
						}
						//*/
					}
				}
				else
				{
					ManterNaArea(reciclavel.transform, area);
					//Som.Tocar(Som.Tipo.ErrarLixeira);
				}
			}
			/*
			else
			{
				ManterNaArea(reciclavel.transform, area);
				Debug.Log("Nome: " + instancia.gameObject.name + " - Tipo diferente");
			}
			*/
		}
	}

	static public GameObject EscolherReciclavelAleatorio()
	{
		//int p = Random.Range(0, instancia.objReciclaveis.Length);
		Reciclavel.Tipo tipo = Reciclavel.PegarTipoAleatorio();
		int p = (int) tipo;

		return instancia.objReciclaveis[p];
	}

	static public void CriarReciclavel()
	{
		GameObject reciclavel	= Instantiate<GameObject>(
			EscolherReciclavelAleatorio());

		reciclavel.transform.SetParent(instancia.transform, false);
		
		float x = Random.Range(0f,1f) * instancia.area.width +
			instancia.area.x;
		float y = Random.Range(0f,1f) * instancia.area.height +
			instancia.area.y;
		
		reciclavel.transform.localPosition = new Vector2(x,y);

		ManterNaArea(
			reciclavel.transform,
			reciclavel.GetComponent<RectTransform>().sizeDelta);
	}

	static public void CriarReciclavel(Reciclavel.Tipo tipo)
	{
		GameObject reciclavel	= Instantiate<GameObject>(
			instancia.objReciclaveis[(int) tipo]);
		
		reciclavel.transform.SetParent(instancia.transform, false);
		
		float x = Random.Range(0f,1f) * instancia.area.width +
			instancia.area.x;
		float y = Random.Range(0f,1f) * instancia.area.height +
			instancia.area.y;
		
		reciclavel.transform.localPosition = new Vector2(x,y);
		
		ManterNaArea(
			reciclavel.transform,
			reciclavel.GetComponent<RectTransform>().sizeDelta);
	}

	static public int QuantidadeEasterEggs()
	{
		if (instancia == null)
		{
			return 0;
		}

		return instancia.objEasterEggs.Length;
	}

	static public void SimularMonstroDerrotado()
	{
		if (Jogador.nivel >= Dados.nivelMinimoEasterEggs && 
			Random.value <= Dados.chanceEasterEgg)
		{
			ObjGerenciadorLixo.CriarReciclavelEasterEgg();
			Debug.Log("CRIOU EASTER ALEATORIO");
		}
		//ObjGerenciadorLixo.CriarReciclavel();
		//ObjGerenciadorLixo.CriarReciclavel();
	}

	// JEF
	static public void CriarReciclavelEasterEgg(int easteregg = -1, bool aleatorio = true)
	{
		if (instancia == null)
		{
			Debug.Log("INSTANCIA NAO ESTA AQUI!!!");
			return;
		}

		if (Jogador.nivel < Dados.nivelMinimoEasterEggs)
		{
			return;
		}

		int i = 0;
		int maxeasteregg = 0;
		List<int> eastereegsAbertos = new List<int>();
		eastereegsAbertos.Clear();
		foreach(Empreendimento e in GerenciadorEmpreendimentos.dicionarioEmpreendimentos.Values)
		{
			if (e.valores.NivelRequisito(1) == -1)
			{
				if (Jogador.nivel >= e.valores.NivelRequisito(0) && e.nivel > 0)
				{
					eastereegsAbertos.Add(e.valores.ID());
				}
			}
		}
		maxeasteregg = eastereegsAbertos.Count;

		if (easteregg >= maxeasteregg) easteregg = maxeasteregg - 1;
		if (easteregg < 0) easteregg = 0;

		int rand = Random.Range(0, maxeasteregg);

		if (eastereegsAbertos.Count > 0)
			i = eastereegsAbertos[rand];
		if (aleatorio == false)
			i = easteregg;

		Debug.Log("Easter Egg Criado, tipo: "+ i);

		GameObject reciclavel;
		if (instancia.objEasterEggs.Length > i)
		{
			Debug.Log("entrou aqui");
			reciclavel	= Instantiate<GameObject>(instancia.objEasterEggs[i]);
			Debug.Log("entrou aqui 2");

			reciclavel.GetComponent<ObjReciclavel>().EasterEgg(i + 1);
			Debug.Log("entrou aqui 3");

			reciclavel.transform.SetParent(instancia.transform, false);

			float x = Random.Range(0f,1f) * instancia.area.width +
				instancia.area.x;
			float y = Random.Range(0f,1f) * instancia.area.height +
				instancia.area.y;

			reciclavel.transform.localPosition = new Vector2(x,y);

			ManterNaArea(
				reciclavel.transform,
				reciclavel.GetComponent<RectTransform>().sizeDelta);
		}


	}

	static public void CriarReciclavel(int tipo)
	{
		if (tipo > 3 || tipo < 0)
		{
			CriarReciclavelEasterEgg();
			return;
		}

		GameObject reciclavel	= Instantiate<GameObject>(
			instancia.objReciclaveis[tipo]);
		
		reciclavel.transform.SetParent(instancia.transform, false);
		
		float x = Random.Range(0f,1f) * instancia.area.width +
			instancia.area.x;
		float y = Random.Range(0f,1f) * instancia.area.height +
			instancia.area.y;
		
		reciclavel.transform.localPosition = new Vector2(x,y);
		
		ManterNaArea(
			reciclavel.transform,
			reciclavel.GetComponent<RectTransform>().sizeDelta);
	}

	static public void CriarXP(
		int quantidade, Transform local, bool positivo = true)
	{
		quantidade = Jogador.XPAjeitada(quantidade);

		if (quantidade > 0)
		{
			quantidade = ObjEmpreendimentos.
				QuantidadeXPAlterada(quantidade);
		}

		Jogador.GanharXP(quantidade);

		GameObject objeto = instancia.objXPNegativo;

		string texto = "" + quantidade + "xp";

		if (positivo)
		{
			objeto = instancia.objXPPositivo;
			texto = "+"+texto;
		}

		Instantiate<GameObject>(objeto).
			GetComponent<ObjTextoFlutuante>().Criar(
				texto, local.position);
	}

	static public void CriarTextoDano(
		int dano, Transform local, int tipo = 0)
	{
		GameObject objeto = instancia.objDanoJogador;
		string texto = "" + dano;

		Instantiate<GameObject>(objeto).
				GetComponent<ObjTextoFlutuante>().Criar(
					texto, local.position);
	}
}

