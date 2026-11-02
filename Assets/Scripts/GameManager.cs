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

    // Botão que aparece quando as definições
    // foram abertas pelo INÍCIO.
    public Button voltarInicioButton;

    // Botão que aparece quando as definições
    // foram abertas pelo JOGO.
    public Button voltarJogoButton;


    // ============================================================
    // SOM
    // ============================================================

    // AudioSource que contém a música.
    public AudioSource audioSource;

    // Slider do volume.
    public Slider volumeSlider;


    // ============================================================
    // LUMINOSIDADE
    // ============================================================

    // Slider da luminosidade.
    public Slider luzSlider;

    // A imagem preta que já tens na Canvas.
    public Image brightnessOverlay;


    // ============================================================
    // VARIÁVEL PARA SABER DE ONDE VIERAM AS DEFINIÇÕES
    // ============================================================

    // false = vieram do início
    // true  = vieram do jogo
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

        // Valores do slider.
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;

        // Começa com volume máximo.
        volumeSlider.value = 1f;

        // Tirar ligação antiga.
        volumeSlider.onValueChanged.RemoveListener(AlterarVolume);

        // Ligar pelo código.
        volumeSlider.onValueChanged.AddListener(AlterarVolume);

        // Aplicar volume inicial.
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

        // Valores do slider.
        luzSlider.minValue = 0f;
        luzSlider.maxValue = 1f;

        // IMPORTANTE:
        // Começa a zero.
        //
        // 0 = completamente transparente.
        // 1 = preto completo.
        luzSlider.value = 0f;

        // Tirar ligação antiga.
        luzSlider.onValueChanged.RemoveListener(
            AlterarLuminosidade
        );

        // Ligar pelo código.
        luzSlider.onValueChanged.AddListener(
            AlterarLuminosidade
        );

        // Aplicar luminosidade inicial.
        AlterarLuminosidade(0f);
    }


    // ============================================================
    // CONFIGURAR BOTÕES DAS DEFINIÇÕES
    // ============================================================

    private void ConfigurarBotoesDefinicoes()
    {
        // --------------------------------------------------------
        // BOTÃO VOLTAR AO INÍCIO
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // BOTÃO VOLTAR AO JOGO
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // NO INÍCIO, O BOTÃO "VOLTAR AO INÍCIO"
        // É O ÚNICO QUE DEVE ESTAR VISÍVEL.
        // --------------------------------------------------------

        if (voltarInicioButton != null)
        {
            voltarInicioButton.gameObject.SetActive(true);
        }

        if (voltarJogoButton != null)
        {
            voltarJogoButton.gameObject.SetActive(false);
        }
    }


    // ============================================================
    // MOSTRAR INÍCIO
    // ============================================================

    public void MostrarInicio()
    {
        if (inicioPanel != null)
        {
            inicioPanel.SetActive(true);
        }

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(false);
        }

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(false);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(false);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(false);
        }

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }

        // As próximas definições serão consideradas
        // como vindas do início.
        definicoesVieramDoJogo = false;

        // Preparar os botões das definições.
        if (voltarInicioButton != null)
        {
            voltarInicioButton.gameObject.SetActive(true);
        }

        if (voltarJogoButton != null)
        {
            voltarJogoButton.gameObject.SetActive(false);
        }
    }


    // ============================================================
    // MOSTRAR EXPLICAÇÃO
    // ============================================================

    public void MostrarExplicacao()
    {
        if (inicioPanel != null)
        {
            inicioPanel.SetActive(false);
        }

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(true);
        }

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(false);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(false);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(false);
        }

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }
    }


    // ============================================================
    // MOSTRAR JOGO
    // ============================================================

    public void MostrarJogo()
    {
        if (inicioPanel != null)
        {
            inicioPanel.SetActive(false);
        }

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(false);
        }

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(true);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(false);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(false);
        }

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }

        // As próximas definições serão consideradas
        // como vindas do jogo.
        definicoesVieramDoJogo = true;

        // Preparar os botões das definições.
        if (voltarInicioButton != null)
        {
            voltarInicioButton.gameObject.SetActive(false);
        }

        if (voltarJogoButton != null)
        {
            voltarJogoButton.gameObject.SetActive(true);
        }
    }


    // ============================================================
    // MOSTRAR VITÓRIA
    // ============================================================

    public void MostrarVitoria()
    {
        if (inicioPanel != null)
        {
            inicioPanel.SetActive(false);
        }

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(false);
        }

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(false);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(true);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(false);
        }

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }
    }


    // ============================================================
    // MOSTRAR DERROTA
    // ============================================================

    public void MostrarDerrota()
    {
        if (inicioPanel != null)
        {
            inicioPanel.SetActive(false);
        }

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(false);
        }

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(false);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(false);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(true);
        }

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }
    }


    // ============================================================
    // ABRIR DEFINIÇÕES A PARTIR DO INÍCIO
    // ============================================================

    public void AbrirDefinicoesInicio()
    {
        // MARCAR:
        // Estas definições vieram do início.
        definicoesVieramDoJogo = false;


        // --------------------------------------------------------
        // MOSTRAR DEFINIÇÕES
        // --------------------------------------------------------

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(true);
        }


        // --------------------------------------------------------
        // ESCONDER INÍCIO
        // --------------------------------------------------------

        if (inicioPanel != null)
        {
            inicioPanel.SetActive(false);
        }


        // --------------------------------------------------------
        // ESCONDER RESTO
        // --------------------------------------------------------

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(false);
        }

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(false);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(false);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }


        // ========================================================
        // AQUI ESTÁ A CORREÇÃO
        // ========================================================

        // Mostrar:
        // VOLTAR AO INÍCIO
        if (voltarInicioButton != null)
        {
            voltarInicioButton.gameObject.SetActive(true);
        }

        // Esconder:
        // VOLTAR AO JOGO
        if (voltarJogoButton != null)
        {
            voltarJogoButton.gameObject.SetActive(false);
        }
    }


    // ============================================================
    // ABRIR DEFINIÇÕES A PARTIR DO JOGO
    // ============================================================

    public void AbrirDefinicoesJogo()
    {
        // MARCAR:
        // Estas definições vieram do jogo.
        definicoesVieramDoJogo = true;


        // --------------------------------------------------------
        // MOSTRAR DEFINIÇÕES
        // --------------------------------------------------------

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(true);
        }


        // --------------------------------------------------------
        // ESCONDER JOGO
        // --------------------------------------------------------

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(false);
        }


        // --------------------------------------------------------
        // ESCONDER RESTO
        // --------------------------------------------------------

        if (inicioPanel != null)
        {
            inicioPanel.SetActive(false);
        }

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(false);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(false);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }


        // ========================================================
        // AQUI ESTÁ A OUTRA CORREÇÃO
        // ========================================================

        // Esconder:
        // VOLTAR AO INÍCIO
        if (voltarInicioButton != null)
        {
            voltarInicioButton.gameObject.SetActive(false);
        }

        // Mostrar:
        // VOLTAR AO JOGO
        if (voltarJogoButton != null)
        {
            voltarJogoButton.gameObject.SetActive(true);
        }
    }


    // ============================================================
    // VOLTAR DAS DEFINIÇÕES PARA O INÍCIO
    // ============================================================

    public void VoltarParaInicio()
    {
        definicoesVieramDoJogo = false;

        MostrarInicio();
    }


    // ============================================================
    // VOLTAR DAS DEFINIÇÕES PARA O JOGO
    // ============================================================

    public void VoltarParaJogo()
    {
        definicoesVieramDoJogo = true;

        MostrarJogo();
    }


    // ============================================================
    // VOLTAR AUTOMATICAMENTE
    // ============================================================

    public void VoltarDasDefinicoes()
    {
        if (definicoesVieramDoJogo)
        {
            MostrarJogo();
        }
        else
        {
            MostrarInicio();
        }
    }


    // ============================================================
    // MOSTRAR CRÉDITOS
    // ============================================================

    public void MostrarCreditos()
    {
        if (inicioPanel != null)
        {
            inicioPanel.SetActive(false);
        }

        if (explicacaoPanel != null)
        {
            explicacaoPanel.SetActive(false);
        }

        if (jogoPanel != null)
        {
            jogoPanel.SetActive(false);
        }

        if (vitoriaPanel != null)
        {
            vitoriaPanel.SetActive(false);
        }

        if (derrotaPanel != null)
        {
            derrotaPanel.SetActive(false);
        }

        if (definicoesPanel != null)
        {
            definicoesPanel.SetActive(false);
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(true);
        }
    }


    // ============================================================
    // VOLTAR DOS CRÉDITOS
    // ============================================================

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

        Debug.Log(
            "PIMBA CRUSH - Volume: " + valor
        );
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

        // Preto com transparência.
        //
        // 0 = transparente
        // 1 = preto
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
        Debug.Log(
            "PIMBA CRUSH - A sair do jogo..."
        );

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
        MostrarJogo();
    }


    // ============================================================
    // BOTÃO VOLTAR AO INÍCIO
    // ============================================================

    public void VoltarAoInicio()
    {
        MostrarInicio();
    }


    // ============================================================
    // BOTÃO VOLTAR AO JOGO
    // ============================================================

    public void VoltarAoJogo()
    {
        MostrarJogo();
    }


    // ============================================================
    // VERIFICAR REFERÊNCIAS
    // ============================================================

    public void VerificarReferencias()
    {
        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "PIMBA CRUSH - VERIFICAÇÃO"
        );

        Debug.Log(
            "========================================"
        );


        // --------------------------------------------------------
        // PAINÉIS
        // --------------------------------------------------------

        if (inicioPanel == null)
        {
            Debug.LogError("FALTA: InicioPanel");
        }
        else
        {
            Debug.Log("OK: InicioPanel");
        }


        if (explicacaoPanel == null)
        {
            Debug.LogError("FALTA: ExplicacaoPanel");
        }
        else
        {
            Debug.Log("OK: ExplicacaoPanel");
        }


        if (jogoPanel == null)
        {
            Debug.LogError("FALTA: JogoPanel");
        }
        else
        {
            Debug.Log("OK: JogoPanel");
        }


        if (vitoriaPanel == null)
        {
            Debug.LogError("FALTA: VitoriaPanel");
        }
        else
        {
            Debug.Log("OK: VitoriaPanel");
        }


        if (derrotaPanel == null)
        {
            Debug.LogError("FALTA: DerrotaPanel");
        }
        else
        {
            Debug.Log("OK: DerrotaPanel");
        }


        if (definicoesPanel == null)
        {
            Debug.LogError("FALTA: DefinicoesPanel");
        }
        else
        {
            Debug.Log("OK: DefinicoesPanel");
        }


        if (creditosPanel == null)
        {
            Debug.LogError("FALTA: CreditosPanel");
        }
        else
        {
            Debug.Log("OK: CreditosPanel");
        }


        // --------------------------------------------------------
        // SOM
        // --------------------------------------------------------

        if (audioSource == null)
        {
            Debug.LogError("FALTA: AudioSource");
        }
        else
        {
            Debug.Log("OK: AudioSource");
        }


        if (volumeSlider == null)
        {
            Debug.LogError("FALTA: VolumeSlider");
        }
        else
        {
            Debug.Log("OK: VolumeSlider");
        }


        // --------------------------------------------------------
        // LUMINOSIDADE
        // --------------------------------------------------------

        if (luzSlider == null)
        {
            Debug.LogError("FALTA: LuzSlider");
        }
        else
        {
            Debug.Log("OK: LuzSlider");
        }


        if (brightnessOverlay == null)
        {
            Debug.LogError("FALTA: BrightnessOverlay");
        }
        else
        {
            Debug.Log("OK: BrightnessOverlay");
        }


        // --------------------------------------------------------
        // BOTÕES
        // --------------------------------------------------------

        if (voltarInicioButton == null)
        {
            Debug.LogError("FALTA: VoltarInicioButton");
        }
        else
        {
            Debug.Log("OK: VoltarInicioButton");
        }


        if (voltarJogoButton == null)
        {
            Debug.LogError("FALTA: VoltarJogoButton");
        }
        else
        {
            Debug.Log("OK: VoltarJogoButton");
        }


        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "VERIFICAÇÃO TERMINADA"
        );

        Debug.Log(
            "========================================"
        );
    }


    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {
        // Não precisamos de Update.
        // Os sliders funcionam através dos eventos.
    }
}
