using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.AdminPanel.Controllers;
using Buran.Core.MvcLibrary.AdminPanel.Controls;
using Buran.Core.MvcLibrary.Extenders;
using Buran.Core.MvcLibrary.LogUtil;
using Buran.Core.MvcLibrary.Repository;
using Buran.Core.MvcLibrary.Resource;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.AdminPanel.Controllers2
{
    public class List2Controller<T, U> : AdminController
        where T : class
        where U : class
    {
        protected U _context;
        public ResourceManager ResourceMan;
        protected IGenericRepositoryAsync<T> Repo;
        protected string DeleteGoBackUrl = "";
        protected string DeleteJsAction = "ReloadF();";
        protected string DeleteJsAction2 = "Reload('{0}');";

        protected bool PopupEditor { get; set; }
        public EditorPageMenu PageMenu { get; set; }

        #region MENU
        protected void BuildCreateMenu()
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Back,
                Title = UI.Back,
                IconClass = "fas fa-angle-left",
                Url = Url.Action("Index"),
                ButtonClass = "btn btn-default btn-sm"
            });
            var saveButton = new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Save,
                Title = UI.Save,
                IconClass = "fas fa-check",
                Id = "btnFormSubmit",
                ButtonClass = "btn btn-primary btn-sm",
                ButtonIdClass = "btnFormSubmit"
            };
            saveButton.SplitItems.Add(new EditorPageMenuSplitItem
            {
                Title = UI.SaveEdit,
                Id = "btnFormSubmitEdit",
                ButtonClass = "btnFormSubmitEdit",
            });
            saveButton.SplitItems.Add(new EditorPageMenuSplitItem
            {
                Title = UI.SaveNew,
                Id = "btnFormSubmitNew",
                ButtonClass = "btnFormSubmitEdit"
            });
            PageMenu.Items.Add(saveButton);

        }
        protected void BuildEditMenu()
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Back,
                Title = UI.Back,
                IconClass = "fas fa-angle-left",
                Url = Url.Action("Index"),
                ButtonClass = "btn btn-default btn-sm"
            });
            var saveButton = new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Save,
                Title = UI.Save,
                IconClass = "fas fa-check",
                Id = "btnFormSubmit",
                ButtonClass = "btn btn-primary btn-sm",
                ButtonIdClass = "btnFormSubmit"
            };
            saveButton.SplitItems.Add(new EditorPageMenuSplitItem
            {
                Title = UI.SaveEdit,
                Id = "btnFormSubmitEdit",
                ButtonClass = "btnFormSubmitEdit"
            });
            PageMenu.Items.Add(saveButton);
        }
        protected void BuildShowMenu(object id)
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Back,
                Title = UI.Back,
                IconClass = "fas fa-angle-left",
                Url = Url.Action("Index"),
                ButtonClass = "btn btn-default btn-sm"
            });
            if (OnEditAuthCheck())
            {
                PageMenu.Items.Add(new EditorPageMenuItem
                {
                    ItemType = EditPageMenuItemType.Edit,
                    Title = UI.Edit,
                    IconClass = "fas fa-edit",
                    Url = Url.Action("Edit", new { id }),
                    ButtonClass = "btn btn-primary btn-sm"
                });
            }
            if (OnDeleteAuthCheck())
            {
                PageMenu.Items.Add(new EditorPageMenuItem
                {
                    ItemType = EditPageMenuItemType.Delete,
                    Title = UI.Delete,
                    IconClass = "fas fa-trash",
                    Id = "btnFormDelete",
                    PostUrl = Url.Action("Delete", new { id, grid = "goback" }),
                    ConfirmText = UI.ConfirmDelete,
                    ButtonClass = "btn btn-danger btn-sm",
                    ButtonIdClass = "btnFormDelete"
                });
            }
        }
        #endregion

        protected List2Controller(bool popupEditor, U context)
        {
            _context = context;
            PopupEditor = popupEditor;
            PageMenu = new EditorPageMenu();
        }


        protected enum TitleType
        {
            List,
            Create,
            Editor,
            Show
        }
        protected virtual string GetTitleTableName()
        {
            return typeof(T).Name;
        }
        protected string GetTitle(TitleType type)
        {
            var r = GetTitleTableName();
            var suffixName = "";
            if (type == TitleType.List)
            {
                r += "_ListTitle";
                suffixName = UI.TitleList;
            }
            else if (type == TitleType.Create)
            {
                r += "_EditorTitle";
                suffixName = UI.TitleCreate;
            }
            else if (type == TitleType.Editor)
            {
                r += "_EditorTitle";
                suffixName = UI.TitleEdit;
            }
            else if (type == TitleType.Show)
            {
                r += "_EditorTitle";
                suffixName = UI.TitleShow;
            }
            if (ResourceMan != null)
            {
                var rsm = ResourceMan.GetString(r);
                return (rsm.IsEmpty() ? r : rsm) + suffixName;
            }
            return string.Empty;
        }


        public virtual bool OnIndexAuthCheck()
        {
            return true;
        }
        public virtual void OnIndex(int? subId = null)
        {
            ViewBag.ParentId = subId;
        }
        public virtual IActionResult Index(int? subId = null)
        {
            if (!OnIndexAuthCheck())
                return new ForbidResult();
            if (ViewBag.Title == null)
                ViewBag.Title = GetTitle(TitleType.List);
            ViewBag.PopupEditor = PopupEditor;
            OnIndex(subId);
            ViewBag.PageMenu = PageMenu;
            return View();
        }


        public virtual IQueryable OnListDataLoad(int? subId = null)
        {
            return Repo.GetList();
        }
        public virtual IActionResult ListView(int? subId = null)
        {
            if (!OnIndexAuthCheck())
                return new ForbidResult();
            ViewBag.CanCreateAuth = OnCreateAuthCheck();
            ViewBag.CanEditAuth = OnEditAuthCheck();
            ViewBag.CanDeleteAuth = OnDeleteAuthCheck();
            ViewBag.ParentId = subId;
            var model = OnListDataLoad(subId);
            return PartialView(model);
        }


        public virtual void OnIndex2(string subId = null)
        {
        }
        public virtual IActionResult Index2(string subId = null)
        {
            if (!OnIndexAuthCheck())
                return new ForbidResult();
            if (ViewBag.Title == null)
                ViewBag.Title = GetTitle(TitleType.List);
            ViewBag.ParentId = subId;
            OnIndex2(subId);
            ViewBag.PageMenu = PageMenu;
            ViewBag.PopupEditor = PopupEditor;
            return View();
        }


        public virtual IQueryable OnListDataLoad2(string subId = null)
        {
            return Repo.GetList();
        }
        public virtual IActionResult ListView2(string subId = null)
        {
            if (!OnIndexAuthCheck())
                return new ForbidResult();
            ViewBag.ParentId = subId;
            var model = OnListDataLoad2(subId);
            return PartialView(model);
        }


        public virtual bool OnDeleteAuthCheck()
        {
            return true;
        }
        public virtual bool OnDeleteCheck(T item)
        {
            return true;
        }
        public virtual void OnAfterDelete(int id)
        {
        }
        public virtual async Task<T> GetDeleteItem(int id)
        {
            return await Repo.GetItemAsync(id);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id, string grid)
        {
            if (!OnDeleteAuthCheck())
                return new ForbidResult();
            try
            {
                var item = await GetDeleteItem(id);
                if (item == null)
                    return new JavascriptResult(StringExt.JsAlert(UI.NotFound));

                DeleteGoBackUrl = Url.Action("Index");
                if (OnDeleteCheck(item))
                {
                    if (await Repo.DeleteAsync(item))
                    {
                        OnAfterDelete(id);
                        if (!grid.IsEmpty())
                        {
                            if (grid == "goback")
                                return new JavascriptResult($"Goto('{DeleteGoBackUrl}')");
                            else
                                return new JavascriptResult(string.Format(DeleteJsAction2, grid));
                        }
                        else
                            return new JavascriptResult(DeleteJsAction);
                    }
                    return new JavascriptResult(StringExt.JsAlert(MvcLogger.GetErrorMessage(ModelState)));
                }
                return new JavascriptResult(StringExt.JsAlert(UI.NoAccess));
            }
            catch (Exception ex)
            {
                return new JavascriptResult(StringExt.JsAlert(MvcLogger.GetErrorMessage(ex)));
            }
        }


        public virtual bool OnCreateAuthCheck()
        {
            return true;
        }
        public virtual bool OnEditAuthCheck()
        {
            return true;
        }
        public virtual bool OnShowAuthCheck()
        {
            return true;
        }
    }
}
