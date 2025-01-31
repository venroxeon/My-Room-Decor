using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI_ItemSpawner : MonoBehaviour
{
    [SerializeField] Color defItemTypeBtnColor;
    [SerializeField] Color pressedItemTypeBtnColor;

    [SerializeField] List<ItemListStruct> listItemListStruct;

    int indexX;
    float prevX;

    VisualElement root, prevElem = null;
    EntityManager EM;

    List<VisualElement> listItemBtnImageList = new();

    public void OnEnable()
    {
        EM = World.DefaultGameObjectInjectionWorld.EntityManager;
        root = GetComponent<UIDocument>().rootVisualElement;

        RegisterItemSpawnBtn();

        root.Q("ResetBtn").RegisterCallback<PointerCaptureEvent>((evt) => ResetBtn());
    }

    void RegisterItemSpawnBtn()
    {
        int x = 0;
        List<Button> list = root.Query<Button>("ItemTypeBtn").ToList();

        list[0].EnableInClassList("item_type_btn_active", true);
        prevElem = list[0];
        foreach (var itemTypeBtn in list)
        {
            int _x = x++;
            itemTypeBtn.RegisterCallback<PointerCaptureEvent>(OnEnterItemTypeButton);
            itemTypeBtn.RegisterCallback<PointerUpEvent, int>(OnExitItemTypeButton, _x);
        }

        int y = 0;

        foreach (var itemBtn in root.Query<Button>(className: "item_btn").ToList())
        {
            int _y = y++;
            itemBtn.RegisterCallback<PointerCaptureEvent>((evt) => Spawn(_y));

            listItemBtnImageList.Add(itemBtn.Q("ItemBtnImg"));
        }
    }

    void Spawn(int _y)
    {
        Entity itemManagerComp = EM.CreateEntityQuery(typeof(ItemSpawnerComp)).GetSingletonEntity();
        EM.AddComponentData(itemManagerComp, new ItemIndexComp() { row = indexX, col = _y });
    }

    void OnEnterItemTypeButton(PointerCaptureEvent evt)
    {
        prevX = InputManager.Instance.GetFirstTouchPos(0).x;
    }

    void OnExitItemTypeButton(PointerUpEvent evt, int _indexX)
    {
        VisualElement elem = (VisualElement)evt.target;

        if (elem == prevElem) return;
        if (Mathf.Abs(InputManager.Instance.GetFirstTouchPos(0).x - prevX) < 5)
        {
            elem.EnableInClassList("item_type_btn_active", true);

            prevElem.EnableInClassList("item_type_btn_active", false);
            prevElem = elem;

            ReplaceSprites(_indexX);

            indexX = _indexX;
        }
    }

    void ReplaceSprites(int _indexX)
    {
        int x = 0;

        foreach(var sprite in listItemListStruct[_indexX].listItemSprites)
        {
            StyleBackground styleBg = listItemBtnImageList[x].style.backgroundImage;
            Background bg = styleBg.value;
            bg.sprite = sprite;
            styleBg.value = bg;

            listItemBtnImageList[x++].style.backgroundImage = styleBg;
        }
    }

    public void ResetBtn()
    {
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public struct ItemListStruct
{
    public List<Sprite> listItemSprites;
}