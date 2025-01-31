using UnityEngine;
using UnityEngine.UIElements;

public class UI_ToggleAR : MonoBehaviour
{
	[SerializeField] GameObject baseGameObj, arGameObj;
    
    VisualElement root;
    bool isAR = false;
    
    public void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        RegisterARToggleBtn();
    }
    
    public void Awake()
    {
        baseGameObj.SetActive(true);
        arGameObj.SetActive(false);
    }
    
    void RegisterARToggleBtn()
    {
        Button arBtn = root.Query<Button>("ARToggle");

        arBtn.RegisterCallback<PointerCaptureEvent>(Toggle);
    }
    
    public void Toggle(PointerCaptureEvent evt)
    {
        arGameObj.SetActive(!isAR);
        baseGameObj.SetActive(isAR);
        
        isAR = !isAR;
    }
    
}