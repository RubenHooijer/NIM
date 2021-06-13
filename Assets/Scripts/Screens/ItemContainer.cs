using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemContainer<T, U> where T : ContainerItem<U> {

    public List<T> Items { get; }

    private T template;

    public ItemContainer(T itemTemplate) {
        template = itemTemplate;
        Items = new List<T>();

        itemTemplate.gameObject.SetActive(false);
    }

    public void UpdateContainer(IEnumerable<U> dataCollection) {
        if (dataCollection == null) { return; }

        List<T> oldItemList = new List<T>(Items);
        Items.Clear();
        
        foreach (U data in dataCollection) {
            T item = oldItemList.FirstOrDefault(x => x.Data != null && x.Data.Equals(data));

            if (item != null) {
                item.Setup(data);
                oldItemList.Remove(item);
                Items.Add(item);
            } else {
                T newItem = Object.Instantiate(template, template.transform.parent);
                newItem.Setup(data);
                Items.Add(newItem);
            }
        }

        DestroyItems(oldItemList);
    }

    private void DestroyItems(IEnumerable<T> itemCollection) {
        foreach (T item in itemCollection) {
            item.Dispose();
            Object.Destroy(item.gameObject);
        }
    }

}

public abstract class ContainerItem<T> : MonoBehaviour {

    public T Data { get; private set; }

    public void Setup(T data) {
        gameObject.SetActive(true);
        Data = data;
        OnSetup(data);
    }

    public void Dispose() {
        OnDispose();
    }

    protected virtual void OnSetup(T data) { }
    
    protected virtual void OnDispose() { }

}