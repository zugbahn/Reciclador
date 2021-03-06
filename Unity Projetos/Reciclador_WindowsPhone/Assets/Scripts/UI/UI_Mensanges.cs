using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI_Mensanges : MonoBehaviour
{
	static UI_Mensanges instancia = null;

	public float 	_duracaoMensagem	= 5;

	float duracaoMensagem = 5;

	Text txtMensagem;

	List<string> listaMensagens = new List<string>();

	float tempoMensagem = 0;

	string mensagemAtual = "";

	void Awake()
	{
		instancia	= this;
		txtMensagem = GetComponent<Text>();
		duracaoMensagem = _duracaoMensagem;
	}

	void Update()
	{
		if (Time.time > tempoMensagem)
		{
			VerificarAdicionarProxima();
		}
	}

	void VerificarAdicionarProxima()
	{
		mensagemAtual = "";

		if (listaMensagens.Count > 0)
		{
			bool pegouMensagem = false;

			lock (listaMensagens)
			{
				try
				{
					mensagemAtual = listaMensagens[0];
					listaMensagens.RemoveAt(0);
					pegouMensagem = true;

					Som.Tocar(Som.Tipo.Mensagem);
				}
				catch(UnityException e)
				{
					Debug.Log(e);
				}
			}

			if (pegouMensagem)
			{
				tempoMensagem = Time.time + duracaoMensagem;
			}
		}

		txtMensagem.text = mensagemAtual;
	}

	static public bool AdicionarMensagem(string mensagem, float duracao = -1)
	{
		if (Jogador.tutorialRodando) return false;
		lock(instancia.listaMensagens)
		{
			try 
			{
				if (duracao > 0)
				{
					instancia.duracaoMensagem = duracao;
				}
				else
				{
					instancia.duracaoMensagem = instancia._duracaoMensagem;
				}
				instancia.listaMensagens.Add(mensagem);
				Debug.Log ("Mensagem adicionada: '"+mensagem+"'");
				return true;
			}
			catch(UnityException e)
			{
				Debug.Log (e);
			}
		}
		return false;
	}
}

