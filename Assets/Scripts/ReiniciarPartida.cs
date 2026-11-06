
using UnityEngine;

public class ReiniciarPartida : MonoBehaviour
{
    [Header("Ligar os objetos")]
    public TabuleiroManager tabuleiroManager;
    public GameManager gameManager;

    public void Reiniciar()
    {
        if (gameManager == null)
        {
            Debug.LogError("PIMBA CRUSH: Falta ligar o GameManager!");
            return;
        }

        if (tabuleiroManager == null)
        {
            Debug.LogError("PIMBA CRUSH: Falta ligar o TabuleiroManager!");
            return;
        }

        // DESCARTAR A PARTIDA ANTERIOR
        tabuleiroManager.ReiniciarJogo();

        // VOLTAR AO MENU INICIAL
        gameManager.MostrarInicio();

        Debug.Log("PIMBA CRUSH: Partida anterior descartada! 0 pontos e 45 movimentos.");
    }
}



