using TMPro;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class VRTKOVRAvatarUNetJoinButton : MonoBehaviour
{
    // Button text.
    TextMeshProUGUI buttonText;

    // The match associated with this button.
    MatchInfoSnapshot internetMatch;
    VRBLANConnectionInfo lanMatch;





    /**
     * Called when the script awakes.
     * 
     * Button text and click handler are inited.
     * 
     **/
    private void Awake()
    {
        // Get the button text.
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // When clicked, call JoinMatch.
        GetComponent<Button>().onClick.AddListener(JoinMatch);
    }





    /**
     * Called after the button has been instantiated to initialize the button.
     * 
     * The button's text, parent, and transform are initialized.
     * 
     **/
    public void Initialize(MatchInfoSnapshot internetMatch, VRBLANConnectionInfo lanMatch, Transform panelTransform)
    {
        this.internetMatch = internetMatch;
        this.lanMatch = lanMatch;
        if (null != internetMatch)
        {
            buttonText.text = internetMatch.name;
        }
        else
        {
            buttonText.text = lanMatch.name;
        }
        transform.SetParent(panelTransform);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }





    /**
     * Called when the button is clicked.
     * 
     **/
    void JoinMatch()
    {
        // Call JoinMatch on the existing instance of the VRBNetworkManager.
        VRBNetworkManager.Instance.JoinMatch(internetMatch, lanMatch);
    }
	
}
