
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Peca : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    public int linha;
    public int coluna;

    private TabuleiroManager tabuleiroManager;
    private Vector2 posicaoInicial;
    private bool pressionada = false;

    public void Configurar(
        TabuleiroManager manager,
        int novaLinha,
        int novaColuna
    )
    {
        tabuleiroManager = manager;
        linha = novaLinha;
        coluna = novaColuna;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tabuleiroManager == null)
            return;

        posicaoInicial = eventData.position;
        pressionada = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!pressionada)
            return;

        pressionada = false;

        if (tabuleiroManager == null)
            return;

        Vector2 movimento =
            eventData.position - posicaoInicial;

        int direcaoLinha = 0;
        int direcaoColuna = 0;

        if (Mathf.Abs(movimento.x) >
            Mathf.Abs(movimento.y))
        {
            if (movimento.x > 30f)
                direcaoColuna = 1;
            else if (movimento.x < -30f)
                direcaoColuna = -1;
        }
        else
        {
            if (movimento.y > 30f)
                direcaoLinha = -1;
            else if (movimento.y < -30f)
                direcaoLinha = 1;
        }

        if (direcaoLinha != 0 || direcaoColuna != 0)
        {
            tabuleiroManager.ArrastarPeca(
                this,
                direcaoLinha,
                direcaoColuna
            );
        }
        else
        {
            tabuleiroManager.ClicarNaPeca(this);
        }
    }

    // ANIMAÇÃO DE DESAPARECIMENTO
    public IEnumerator FadeOut(float duracao)
    {
        CanvasGroup grupo = GetComponent<CanvasGroup>();

        if (grupo == null)
            grupo = gameObject.AddComponent<CanvasGroup>();

        grupo.interactable = false;
        grupo.blocksRaycasts = false;

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            grupo.alpha = Mathf.Lerp(
                1f,
                0f,
                Mathf.Clamp01(tempo / duracao)
            );

            yield return null;
        }

        grupo.alpha = 0f;
    }

    // ANIMAÇÃO DE APARECIMENTO
    public IEnumerator FadeIn(float duracao)
    {
        CanvasGroup grupo = GetComponent<CanvasGroup>();

        if (grupo == null)
            grupo = gameObject.AddComponent<CanvasGroup>();

        grupo.alpha = 0f;
        grupo.interactable = true;
        grupo.blocksRaycasts = true;

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            grupo.alpha = Mathf.Lerp(
                0f,
                1f,
                Mathf.Clamp01(tempo / duracao)
            );

            yield return null;
        }

        grupo.alpha = 1f;
    }
}

