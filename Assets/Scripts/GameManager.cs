
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ============================================================
    // PAINÉIS PRINCIPAIS
    // ============================================================

    public GameObject inicioPanel;
    public GameObject explicacaoPanel;
    public GameObject jogoPanel;
    public GameObject vitoriaPanel;
    public GameObject derrotaPanel;

    // ============================================================
    // PAINÉIS EXTRA
    // ============================================================

    public GameObject definicoesPanel;
    public GameObject creditosPanel;

    // ============================================================
    // BOTÕES DAS DEFINIÇÕES
    // ============================================================

    public Button voltarInicioButton;
    public Button voltarJogoButton;

    // ============================================================
    // SOM
    // ============================================================

    // AudioSource que já utilizas para a música e volume.
    public AudioSource audioSource;

    // Slider do volume.
    public Slider volumeSlider;

    // Novos sons de vitória e derrota.
    public AudioClip somVitoria;
    public AudioClip somDerrota;

    // ============================================================
    // LUMINOSIDADE
    // ============================================================

    public Slider luzSlider;
    public Image brightnessOverlay;

    // ============================================================
    // TABULEIRO
    // ============================================================

    public TabuleiroManager tabuleiroManager;

    // ============================================================
    // VARIÁVEL DAS DEFINIÇÕES
    // ============================================================

    // false = vieram do início
    // true = vieram do jogo
    private bool definicoesVieramDoJogo = false;

    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        ConfigurarVolume();
        ConfigurarLuminosidade();
        ConfigurarBotoesDefinicoes();

        MostrarInicio();
    }

    // ============================================================
    // CONFIGURAR VOLUME
    // ============================================================

    private void ConfigurarVolume()
    {
        if (audioSource == null)
        {
            Debug.LogWarning(
                "PIMBA CRUSH: AudioSource não está ligado no GameManager."
            );
        }

        if (volumeSlider == null)
        {
            Debug.LogWarning(
                "PIMBA CRUSH: VolumeSlider não está ligado no GameManager."
            );

            return;
        }

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = 1f;

        volumeSlider.onValueChanged.RemoveListener(AlterarVolume);
        volumeSlider.onValueChanged.AddListener(AlterarVolume);

        AlterarVolume(volumeSlider.value);
    }

    // ============================================================
    // CONFIGURAR LUMINOSIDADE
    // ============================================================

    private void ConfigurarLuminosidade()
    {
        if (luzSlider == null)
        {
            Debug.LogWarning(
                "PIMBA CRUSH: LuzSlider não está ligado no GameManager."
            );

            return;
        }

        luzSlider.minValue = 0f;
        luzSlider.maxValue = 1f;
        luzSlider.value = 0f;

        luzSlider.onValueChanged.RemoveListener(
            AlterarLuminosidade
        );

        luzSlider.onValueChanged.AddListener(
            AlterarLuminosidade
        );

        AlterarLuminosidade(0f);
    }

    // ============================================================
    // CONFIGURAR BOTÕES DAS DEFINIÇÕES
    // ============================================================

    private void ConfigurarBotoesDefinicoes()
    {
        if (voltarInicioButton != null)
        {
            voltarInicioButton.onClick.RemoveListener(
                VoltarParaInicio
            );

            voltarInicioButton.onClick.AddListener(
                VoltarParaInicio
            );
        }
        else
        {
            Debug.LogWarning(
                "PIMBA CRUSH: VoltarInicioButton não está ligado."
            );
        }

        if (voltarJogoButton != null)
        {
            voltarJogoButton.onClick.RemoveListener(
                VoltarParaJogo
            );

            voltarJogoButton.onClick.AddListener(
                VoltarParaJogo
            );
        }
        else
        {
            Debug.LogWarning(
                "PIMBA CRUSH: VoltarJogoButton não está ligado."
            );
        }

        if (voltarInicioButton != null)
            voltarInicioButton.gameObject.SetActive(true);

        if (voltarJogoButton != null)
            voltarJogoButton.gameObject.SetActive(false);
    }

    // ============================================================
    // MOSTRAR INÍCIO
    // ============================================================

    public void MostrarInicio()
    {
        if (inicioPanel != null)
            inicioPanel.SetActive(true);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(false);

        if (jogoPanel != null)
            jogoPanel.SetActive(false);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(false);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(false);

        if (definicoesPanel != null)
            definicoesPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(false);

        definicoesVieramDoJogo = false;

        if (voltarInicioButton != null)
            voltarInicioButton.gameObject.SetActive(true);

        if (voltarJogoButton != null)
            voltarJogoButton.gameObject.SetActive(false);
    }

    // ============================================================
    // MOSTRAR EXPLICAÇÃO
    // ============================================================

    public void MostrarExplicacao()
    {
        if (inicioPanel != null)
            inicioPanel.SetActive(false);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(true);

        if (jogoPanel != null)
            jogoPanel.SetActive(false);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(false);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(false);

        if (definicoesPanel != null)
            definicoesPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(false);
    }

    // ============================================================
    // MOSTRAR JOGO
    // ============================================================

    public void MostrarJogo()
    {
        if (inicioPanel != null)
            inicioPanel.SetActive(false);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(false);

        if (jogoPanel != null)
            jogoPanel.SetActive(true);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(false);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(false);

        if (definicoesPanel != null)
            definicoesPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(false);

        definicoesVieramDoJogo = true;

        if (voltarInicioButton != null)
            voltarInicioButton.gameObject.SetActive(false);

        if (voltarJogoButton != null)
            voltarJogoButton.gameObject.SetActive(true);
    }

    // ============================================================
    // MOSTRAR VITÓRIA
    // ============================================================

    public void MostrarVitoria()
    {
        // Tocar som de vitória.
        if (audioSource != null && somVitoria != null)
        {
            audioSource.PlayOneShot(somVitoria);
        }

        if (inicioPanel != null)
            inicioPanel.SetActive(false);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(false);

        if (jogoPanel != null)
            jogoPanel.SetActive(false);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(true);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(false);

        if (definicoesPanel != null)
            definicoesPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(false);
    }

    // ============================================================
    // MOSTRAR DERROTA
    // ============================================================

    public void MostrarDerrota()
    {
        // Tocar som de derrota.
        if (audioSource != null && somDerrota != null)
        {
            audioSource.PlayOneShot(somDerrota);
        }

        if (inicioPanel != null)
            inicioPanel.SetActive(false);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(false);

        if (jogoPanel != null)
            jogoPanel.SetActive(false);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(false);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(true);

        if (definicoesPanel != null)
            definicoesPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(false);
    }

    // ============================================================
    // ABRIR DEFINIÇÕES A PARTIR DO INÍCIO
    // ============================================================

    public void AbrirDefinicoesInicio()
    {
        definicoesVieramDoJogo = false;

        if (definicoesPanel != null)
            definicoesPanel.SetActive(true);

        if (inicioPanel != null)
            inicioPanel.SetActive(false);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(false);

        if (jogoPanel != null)
            jogoPanel.SetActive(false);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(false);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(false);

        if (voltarInicioButton != null)
            voltarInicioButton.gameObject.SetActive(true);

        if (voltarJogoButton != null)
            voltarJogoButton.gameObject.SetActive(false);
    }

    // ============================================================
    // ABRIR DEFINIÇÕES A PARTIR DO JOGO
    // ============================================================

    public void AbrirDefinicoesJogo()
    {
        definicoesVieramDoJogo = true;

        if (definicoesPanel != null)
            definicoesPanel.SetActive(true);

        if (jogoPanel != null)
            jogoPanel.SetActive(false);

        if (inicioPanel != null)
            inicioPanel.SetActive(false);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(false);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(false);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(false);

        if (voltarInicioButton != null)
            voltarInicioButton.gameObject.SetActive(false);

        if (voltarJogoButton != null)
            voltarJogoButton.gameObject.SetActive(true);
    }

    // ============================================================
    // VOLTAR DAS DEFINIÇÕES
    // ============================================================

    public void VoltarParaInicio()
    {
        definicoesVieramDoJogo = false;
        MostrarInicio();
    }

    public void VoltarParaJogo()
    {
        definicoesVieramDoJogo = true;
        MostrarJogo();
    }

    public void VoltarDasDefinicoes()
    {
        if (definicoesVieramDoJogo)
            MostrarJogo();
        else
            MostrarInicio();
    }

    // ============================================================
    // CRÉDITOS
    // ============================================================

    public void MostrarCreditos()
    {
        if (inicioPanel != null)
            inicioPanel.SetActive(false);

        if (explicacaoPanel != null)
            explicacaoPanel.SetActive(false);

        if (jogoPanel != null)
            jogoPanel.SetActive(false);

        if (vitoriaPanel != null)
            vitoriaPanel.SetActive(false);

        if (derrotaPanel != null)
            derrotaPanel.SetActive(false);

        if (definicoesPanel != null)
            definicoesPanel.SetActive(false);

        if (creditosPanel != null)
            creditosPanel.SetActive(true);
    }

    public void VoltarDosCreditos()
    {
        MostrarInicio();
    }

    // ============================================================
    // ALTERAR VOLUME
    // ============================================================

    public void AlterarVolume(float valor)
    {
        if (audioSource == null)
        {
            Debug.LogWarning(
                "PIMBA CRUSH: AudioSource não está ligado."
            );

            return;
        }

        valor = Mathf.Clamp01(valor);
        audioSource.volume = valor;

        Debug.Log("PIMBA CRUSH - Volume: " + valor);
    }

    // ============================================================
    // ALTERAR LUMINOSIDADE
    // ============================================================

    public void AlterarLuminosidade(float valor)
    {
        if (brightnessOverlay == null)
        {
            Debug.LogError(
                "PIMBA CRUSH: BrightnessOverlay NÃO está ligado no GameManager!"
            );

            return;
        }

        valor = Mathf.Clamp01(valor);

        Color novaCor = new Color(
            0f,
            0f,
            0f,
            valor
        );

        brightnessOverlay.color = novaCor;

        Debug.Log(
            "PIMBA CRUSH - Luminosidade: " + valor +
            " | Alpha: " + brightnessOverlay.color.a
        );
    }

    // ============================================================
    // BOTÃO SAIR
    // ============================================================

    public void SairDoJogo()
    {
        Debug.Log("PIMBA CRUSH - A sair do jogo...");
        Application.Quit();
    }

    // ============================================================
    // BOTÃO JOGAR
    // ============================================================

    public void Jogar()
    {
        MostrarExplicacao();
    }

    // ============================================================
    // BOTÃO INICIAR
    // ============================================================

    public void IniciarJogo()
    {
        if (tabuleiroManager != null)
            tabuleiroManager.ReiniciarJogo();

        MostrarJogo();
    }

    // ============================================================
    // BOTÕES DE NAVEGAÇÃO
    // ============================================================

    public void VoltarAoInicio()
    {
        MostrarInicio();
    }

    public void VoltarAoJogo()
    {
        MostrarJogo();
    }

    // ============================================================
    // VERIFICAR REFERÊNCIAS
    // ============================================================

    public void VerificarReferencias()
    {
        Debug.Log("========================================");
        Debug.Log("PIMBA CRUSH - VERIFICAÇÃO");
        Debug.Log("========================================");

        VerificarObjeto(inicioPanel, "InicioPanel");
        VerificarObjeto(explicacaoPanel, "ExplicacaoPanel");
        VerificarObjeto(jogoPanel, "JogoPanel");
        VerificarObjeto(vitoriaPanel, "VitoriaPanel");
        VerificarObjeto(derrotaPanel, "DerrotaPanel");
        VerificarObjeto(definicoesPanel, "DefinicoesPanel");
        VerificarObjeto(creditosPanel, "CreditosPanel");

        VerificarObjeto(audioSource, "AudioSource");
        VerificarObjeto(volumeSlider, "VolumeSlider");
        VerificarObjeto(luzSlider, "LuzSlider");
        VerificarObjeto(brightnessOverlay, "BrightnessOverlay");

        VerificarObjeto(voltarInicioButton, "VoltarInicioButton");
        VerificarObjeto(voltarJogoButton, "VoltarJogoButton");

        VerificarObjeto(somVitoria, "SomVitoria");
        VerificarObjeto(somDerrota, "SomDerrota");

        VerificarObjeto(tabuleiroManager, "TabuleiroManager");

        Debug.Log("========================================");
        Debug.Log("VERIFICAÇÃO TERMINADA");
        Debug.Log("========================================");
    }

    private void VerificarObjeto(Object objeto, string nome)
    {
        if (objeto == null)
            Debug.LogError("FALTA: " + nome);
        else
            Debug.Log("OK: " + nome);
    }
}

