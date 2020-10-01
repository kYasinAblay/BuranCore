using Buran.Core.Library.Utils;
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

namespace Buran.Core.MvcLibrary.AdminPanel.Controllers
{
    public class ListController<T, Z> : AdminController
        where T : class
        where Z : class
    {
        protected Z _context;
        public ResourceManager ResourceMan;
        protected IGenericRepository<T> Repo;
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
                IconClass = "fa fa-angle-left",
                Url = Url.Action("Index"),
                ButtonClass = "btn btn-default"
            });
            //PageMenu.Items.Add(new EditorPageMenuItem
            //{
            //    ItemType = EditPageMenuItemType.Reset,
            //    Title = UI.Reset,
            //    IconClass = "fa fa-reply",
            //    Url = Url.Action("Create"),
            //    ButtonClass = "btn btn-default"
            //});
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Save,
                Title = UI.Save,
                IconClass = "fa fa-check",
                Id = "btnFormSubmit",
                ButtonClass = "btn blue-steel"
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.SaveContinue,
                Title = UI.SaveEdit,
                IconClass = "fa fa-check-circle",
                Id = "btnFormSubmitEdit",
                ButtonClass = "btn blue-steel"
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.SaveNew,
                Title = UI.SaveNew,
                IconClass = "fa fa-check-circle",
                Id = "btnFormSubmitNew",
                ButtonClass = "btn blue-steel"
            });
        }
        protected void BuildEditMenu(object id)
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Back,
                Title = UI.Back,
                IconClass = "fa fa-angle-left",
                Url = Url.Action("Index"),
                ButtonClass = "btn btn-default"
            });
            //PageMenu.Items.Add(new EditorPageMenuItem
            //{
            //    ItemType = EditPageMenuItemType.Reset,
            //    Title = UI.Reset,
            //    IconClass = "fa fa-reply",
            //    Url = Url.Action("Edit", new { id }),
            //    ButtonClass = "btn btn-default"
            //});
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Save,
                Title = UI.Save,
                IconClass = "fa fa-check",
                Id = "btnFormSubmit",
                ButtonClass = "btn blue-steel"
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.SaveContinue,
                Title = UI.SaveEdit,
                IconClass = "fa fa-check-circle",
                Id = "btnFormSubmitEdit",
                ButtonClass = "btn blue-steel"
            });
        }
        protected void BuildShowMenu(object id)
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Back,
                Title = UI.Back,
                IconClass = "fa fa-angle-left",
                Url = Url.Action("Index"),
                ButtonClass = "btn btn-default"
            });
            if (OnEditAuthCheck())
            {
                PageMenu.Items.Add(new EditorPageMenuItem
                {
                    ItemType = EditPageMenuItemType.Edit,
                    Title = UI.Edit,
                    IconClass = "fa fa-pencil",
                    Url = Url.Action("Edit", new { id }),
                    ButtonClass = "btn green-haze"
                });
            }
            if (OnDeleteAuthCheck())
            {
                PageMenu.Items.Add(new EditorPageMenuItem
                {
                    ItemType = EditPageMenuItemType.Delete,
                    Title = UI.Delete,
                    IconClass = "fa fa-trash-o",
                    Id = "btnFormDelete",
                    PostUrl = Url.Action("Delete", new { id, grid = "goback" }),
                    ConfirmText = UI.ConfirmDelete,
                    ButtonClass = "btn red"
                });
            }
        }
        #endregion

        protected ListController(bool popupEditor, Z context)
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
        [HttpPost]
        public virtual IActionResult Delete(int id, string grid)
        {
            if (!OnDeleteAuthCheck())
                return new ForbidResult();
            try
            {
                var item = Repo.GetItem(id);
                if (item == null)
                    return new JavascriptResult(StringExt.JsAlert(UI.NotFound));

                DeleteGoBackUrl = Url.Action("Index");
                if (OnDeleteCheck(item))
                {
                    if (Repo.Delete(item))
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
