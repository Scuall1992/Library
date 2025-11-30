using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputVariant : BaseBoot
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private FpsMovement fpsMovement;
    [SerializeField] public BallController ballController;
    [SerializeField] private TextMeshProUGUI _type;

    public async override UniTask Boot()
    {
        await base.Boot();

        
        

        toggle.onValueChanged.AddListener(delegate {
            ChangeToggle();
        });
    }

    private void ChangeToggle()
    {
        if (toggle.isOn)
        {
            _type.text = "Шар";
            ballController.isBall = true;
        }
        else
        {
            _type.text = "Платформа";
            ballController.isBall = false;
        }
    }
}
