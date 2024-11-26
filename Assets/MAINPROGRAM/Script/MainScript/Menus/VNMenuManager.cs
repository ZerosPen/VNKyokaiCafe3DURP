    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;

    public class VNMenuManager : MonoBehaviour
    {
        private MenuPage activePage = null;
        private bool isOpen = false;

        [SerializeField] private CanvasGroup root;
        [SerializeField] private MenuPage[] pages;

        private CanvasGroupController rootCG ;

        // Start is called before the first frame update
        void Start()
        {
            rootCG = new CanvasGroupController(this, root);   
        }

        private MenuPage GetPage(MenuPage.PagesType pageType)
        {
            return pages.FirstOrDefault(page => page.pageType == pageType);
        }

        public void OpenConfigPage()
        {
            var page = GetPage(MenuPage.PagesType.Config);
            OpenPage(page);
        }

        public void OpenHelpPage()
        {
            var page = GetPage(MenuPage.PagesType.Help);  
            OpenPage(page);
        }

    private void OpenPage(MenuPage page)
    {
        Debug.Log($"Attempting to open page: {page}");
        if (page == null)
        {
            Debug.LogError("The provided page is null!");
            return;
        }

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            OpenRoot();
    }

    public void OpenRoot()
        {
            rootCG.Show();
            rootCG.SetInteractableState(true);
            isOpen = true;
        }

    public void CLoseRoot()
        {
            rootCG.Hide();
            rootCG.SetInteractableState(false);
            isOpen = false;
        }

    public void Click_Home()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
        
    public void Click_Quit()
    {
        Application.Quit();
    }
}
    