using TMPro;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using VRTK;

/// <summary>
/// This class contains logic for the join match buttons.
/// </summary>
public class VRTKOVRAvatarUNetJoinButton : MonoBehaviour
{

    #region PRIVATE VARIABLES


    /// <summary>
    /// Cached reference to the button text.
    /// </summary>
    TextMeshProUGUI buttonText;

    /// <summary>
    /// The match info for this button.
    /// </summary>
    VRBMatchInfo matchInfo;


    #endregion





    #region MONOBEHAVIOUR METHODS


    /// <summary>
    /// Button text and click handler are inited.
    /// </summary>
    private void Awake()
    {
        // Get the button text.
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // When clicked, call JoinMatch.
        GetComponent<Button>().onClick.AddListener(JoinMatch);
    }





    /// <summary>
    /// Initializes button.
    /// </summary>
    /// <param name="matchInfo">Match info for the button.</param>
    /// <param name="panelTransform">The panel to add the button to.</param>
    public void Initialize(VRBMatchInfo matchInfo, Transform panelTransform)
    {
        this.matchInfo = matchInfo;
        buttonText.text = matchInfo.name;
        transform.SetParent(panelTransform);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }





    /// <summary>
    /// Callback called when the button is clicked.
    /// 
    /// The demo is notified to start the client.
    /// </summary>
    void JoinMatch()
    {
        VRTKOVRAvatarUNetDemo demo = VRTK_SharedMethods.FindEvenInactiveComponent<VRTKOVRAvatarUNetDemo>();
        demo.StartClient(matchInfo);
    }


    #endregion


}
