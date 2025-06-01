using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
   [SerializeField]
   private TextMeshProUGUI _itemName;
   
   [SerializeField]
   private Image _itemIcon;
   
   [SerializeField]
   private GameObject _stackObject;

   [SerializeField] private TextMeshProUGUI _stackNumber;

   public void Set(InventoryItem item)
   {
      _itemName.text = item.data.itemName;
      _itemIcon.sprite = item.data.itemIcon;
      
      if (item.stackSize <= 1)
      {
         _stackObject.SetActive(false); // oculta si no hay más de 1
      }
      else
      {
         _stackObject.SetActive(true);
         _stackNumber.text = item.stackSize.ToString();
      }

      // if (item.stackSize <= 1)
      // {
      //    _stackObject.SetActive(true);
      //    return;
      // }
      
      _stackNumber.text = item.stackSize.ToString();
   }

}
