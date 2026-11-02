using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject inicioPanel;
    public GameObject explicacaoPanel;
    public GameObject jogoPanel;
    public GameObject vitoriaPanel;
    public GameObject derrotaPanel;
    public GameObject definicoesPanel;
    public GameObject creditosPanel;

    void Start()
    {
        MostrarInicio();
    }

    public void MostrarInicio()
    {
        inicioPanel.SetActive(true);
        explicacaoPanel.SetActive(false);
        jogoPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        derrotaPanel.SetActive(false);
        definicoesPanel.SetActive(false);
        creditosPanel.SetActive(false);
    }

    public void MostrarExplicacao()
    {
        inicioPanel.SetActive(false);
        explicacaoPanel.SetActive(true);
        jogoPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        derrotaPanel.SetActive(false);
        definicoesPanel.SetActive(false);
        creditosPanel.SetActive(false);
    }

    public void MostrarJogo()
    {
        inicioPanel.SetActive(false);
        explicacaoPanel.SetActive(false);
        jogoPanel.SetActive(true);
        vitoriaPanel.SetActive(false);
        derrotaPanel.SetActive(false);
        definicoesPanel.SetActive(false);
        creditosPanel.SetActive(false);
    }

    public void MostrarVitoria()
    {
        inicioPanel.SetActive(false);
        explicacaoPanel.SetActive(false);
        jogoPanel.SetActive(false);
        vitoriaPanel.SetActive(true);
        derrotaPanel.SetActive(false);
        definicoesPanel.SetActive(false);
        creditosPanel.SetActive(false);
    }

    public void MostrarDerrota()
    {
        inicioPanel.SetActive(false);
        explicacaoPanel.SetActive(false);
        jogoPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        derrotaPanel.SetActive(true);
        definicoesPanel.SetActive(false);
        creditosPanel.SetActive(false);
    }

    // DEFINIÇÕES VINDO DO INÍCIO
    public void MostrarDefinicoesDoInicio()
    {
        inicioPanel.SetActive(false);
        explicacaoPanel.SetActive(false);
        jogoPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        derrotaPanel.SetActive(false);
        definicoesPanel.SetActive(true);
        creditosPanel.SetActive(false);
    }

    // DEFINIÇÕES VINDO DO JOGO
    public void MostrarDefinicoesDoJogo()
    {
        inicioPanel.SetActive(false);
        explicacaoPanel.SetActive(false);
        jogoPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        derrotaPanel.SetActive(false);
        definicoesPanel.SetActive(true);
        creditosPanel.SetActive(false);
    }

    // VOLTAR DAS DEFINIÇÕES PARA O INÍCIO
    public void VoltarDasDefinicoesParaInicio()
    {
        MostrarInicio();
    }

    // VOLTAR DAS DEFINIÇÕES PARA O JOGO
    public void VoltarDasDefinicoesParaJogo()
    {
        MostrarJogo();
    }

    // CRÉDITOS
    public void MostrarCreditos()
    {
        inicioPanel.SetActive(false);
        explicacaoPanel.SetActive(false);
        jogoPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        derrotaPanel.SetActive(false);
        definicoesPanel.SetActive(false);
        creditosPanel.SetActive(true);
    }

    // VOLTAR DOS CRÉDITOS
    public void VoltarDosCreditos()
    {
        MostrarInicio();
    }

    // SAIR
    public void SairDoJogo()
    {
        Application.Quit();
    }
}
