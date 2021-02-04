using Buran.Core.Library.Reflection;
using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.AdminPanel.Controls;
using Buran.Core.MvcLibrary.AdminPanel.Utils;
using Buran.Core.MvcLibrary.LogUtil;
using Buran.Core.MvcLibrary.Reflection;
using Buran.Core.MvcLibrary.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.AdminPanel.Controllers2
{
    public class Edit2Controller<T, U> : List2Controller<T, U>
         where T : class, new()
         where U : class
    {
        protected string ViewEditPopup = "EditPopup";
        protected string ViewEdit = "Edit";
        protected string ViewCreatePopup = "CreatePopup";
        protected string ViewCreate = "Create";

        protected string CreateAction = "Create";
        protected string CreateJsAction = "";
        protected string EditAction = "Edit";
        protected string EditJsAction = "";

        protected string CreateSaveAndCreateUrl = string.Empty;
        protected string CreateReturnListUrl = string.Empty;
        protected string EditReturnListUrl = string.Empty;

        protected Edit2Controller(bool popupEditor, U context)
         : base(popupEditor, context)
        {
            if (popupEditor)
            {
                EditAction = "EditPopup";
                CreateAction = "CreatePopup";
            }
        }

        public override void OnIndex(int? subId = null)
        {
            if (OnCreateAuthCheck())
            {
                PageMenu.Items.Add(new EditorPageMenuItem
                {
                    ItemType = EditPageMenuItemType.Insert,
                    Title = UI.New,
                    IconClass = "fas fa-plus",
                    ButtonClass = "btn btn-primary btn-sm",
                    Url = Url.Action("Create")
                });
            }
            base.OnIndex(subId);
        }

        #region SHOW
        public virtual bool OnShowCheck(T item)
        {
            return true;
        }
        public virtual void OnShowItem(T item)
        {
        }
        public virtual async Task<T> GetShowItem(int id)
        {
            return await Repo.GetItemAsync(id);
        }
        public virtual async Task<IActionResult> Show(int id)
        {
            if (!OnShowAuthCheck())
                return new ForbidResult();
            var item = await GetShowItem(id);
            if (item == null)
                return NotFound();
            if (!OnShowCheck(item))
                return NotFound();

            var _queryDictionary = QueryHelpers.ParseQuery(Request.QueryString.ToString());
            var _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var gridItem = _queryItems.FirstOrDefault(d => d.Key == "grid");
            var grid = "";
            grid = gridItem.Value;

            ViewBag.ShowMode = true;
            ViewBag.Title = GetTitle(TitleType.Show);
            ViewBag.Grid = grid;
            BuildShowMenu(id);

            OnShowItem(item);

            ViewBag.KeyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            ViewBag.KeyFieldValue = id;

            ViewBag.PageMenu = PageMenu;
            return View(item);
        }
        #endregion

        #region CREATE
        public virtual void AddNewItem(T item)
        {
        }
        public virtual bool OnCreateCheck(T item)
        {
            return true;
        }
        public virtual IActionResult Create()
        {
            if (!OnCreateAuthCheck())
                return new ForbidResult();

            var _queryDictionary = QueryHelpers.ParseQuery(Request.QueryString.ToString());
            var _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var gridItem = _queryItems.FirstOrDefault(d => d.Key == "grid");
            var grid = "";
            grid = gridItem.Value;

            ViewBag.EditMode = false;
            ViewBag.CreateAction = CreateAction;
            ViewBag.Title = GetTitle(TitleType.Create);
            ViewBag.Grid = grid;
            BuildCreateMenu();

            var item = new T();
            AddNewItem(item);

            if (!OnCreateCheck(item))
                return NotFound();

            ViewBag.PageMenu = PageMenu;
            return View(PopupEditor ? ViewCreatePopup : ViewCreate, item);
        }


        public virtual void OnCreateSaveItem(T item)
        {
        }
        public virtual void OnAfterCreateSaveItem(T item)
        {
        }
        public virtual void OnErrorCreateSaveItem(T item)
        {
        }


        public virtual bool OnCreateSaveCheck(T item)
        {
            return true;
        }
        [HttpPost]
        public virtual async Task<IActionResult> Create(int keepEdit, T item)
        {
            if (!OnCreateAuthCheck())
                return new ForbidResult();
            var keyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            try
            {
                OnCreateSaveItem(item);
                if (OnCreateSaveCheck(item))
                {
                    if (await Repo.CreateAsync(item))
                    {
                        OnAfterCreateSaveItem(item);
                        if (keepEdit == 0)
                            return CreateReturnListUrl.IsEmpty() ? (ActionResult)RedirectToAction("Index") : Redirect(CreateReturnListUrl);
                        if (keepEdit == 1)
                        {
                            var itemIdValue = Digger.GetObjectValue(item, keyFieldName);
                            return RedirectToAction("Edit", new { id = itemIdValue });
                        }
                        return CreateSaveAndCreateUrl.IsEmpty() ? (ActionResult)RedirectToAction("Create") : Redirect(CreateSaveAndCreateUrl);
                    }
                }
                return new ForbidResult();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(keyFieldName, MvcLogger.GetErrorMessage(ex));
            }
            ViewBag.EditMode = false;
            ViewBag.CreateAction = CreateAction;
            ViewBag.Title = GetTitle(TitleType.Create);
            BuildCreateMenu();
            OnErrorCreateSaveItem(item);
            ViewBag.PageMenu = PageMenu;
            return View(PopupEditor ? ViewCreatePopup : ViewCreate, item);
        }
        [HttpPost]
        public virtual async Task<JsonResult> CreatePopup(T item)
        {
            var r = new JsonResultViewModel();
            if (!OnCreateAuthCheck())
            {
                r.Error = "FORBIDDEN";
                return Json(r);
            }
            try
            {
                OnCreateSaveItem(item);
                if (OnCreateSaveCheck(item))
                {
                    if (await Repo.CreateAsync(item))
                    {
                        OnAfterCreateSaveItem(item);
                        r.Ok = true;
                        if (!CreateJsAction.IsEmpty())
                            r.JsFunction = CreateJsAction;
                    }
                    else
                        r.Error = MvcLogger.GetErrorMessage(ModelState);
                }
            }
            catch (Exception ex)
            {
                r.Error = MvcLogger.GetErrorMessage(ex);
            }
            return Json(r);
        }
        #endregion

        #region EDIT
        public virtual bool OnEditCheck(T item)
        {
            return true;
        }
        public virtual void OnEditItem(T item)
        {
        }
        public virtual async Task<T> GetEditItem(int id)
        {
            return await Repo.GetItemAsync(id);
        }
        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!OnEditAuthCheck())
                return new ForbidResult();

            var item = await GetEditItem(id);
            if (item == null)
                return NotFound();

            if (!OnEditCheck(item))
                return NotFound();

            var _queryDictionary = QueryHelpers.ParseQuery(Request.QueryString.ToString());
            var _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var gridItem = _queryItems.FirstOrDefault(d => d.Key == "grid");
            var grid = "";
            grid = gridItem.Value;

            ViewBag.EditMode = true;
            ViewBag.EditAction = EditAction;
            ViewBag.Title = GetTitle(TitleType.Editor);
            ViewBag.Grid = grid;
            BuildEditMenu();

            OnEditItem(item);

            ViewBag.KeyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            ViewBag.KeyFieldValue = id;

            ViewBag.PageMenu = PageMenu;
            return View(PopupEditor ? ViewEditPopup : ViewEdit, item);
        }


        public virtual void OnEditSaveItem(T item)
        {
        }
        public virtual bool OnEditSaveCheck(T item)
        {
            return true;
        }
        public virtual void OnEditBeforeSaveItem(T item, T dbItem)
        {
        }
        public virtual void OnAfterEditSaveItem(T item)
        {
        }
        private int _editId;

        [HttpPost]
        public virtual async Task<IActionResult> Edit(int keepEdit, T item)
        {
            if (!OnEditAuthCheck())
                return new ForbidResult();

            var keyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            var v = Digger.GetObjectValue(item, keyFieldName);
            if (v != null)
            {
                int.TryParse(v.ToString(), out _editId);
                if (_editId > 0)
                {
                    var org = await Repo.GetItemAsync(_editId);
                    OnEditBeforeSaveItem(item, org);
                    await TryUpdateModelAsync(org);
                    if (OnEditSaveCheck(org))
                    {
                        OnEditSaveItem(org);
                        try
                        {
                            if (await Repo.EditAsync(org))
                            {
                                OnAfterEditSaveItem(org);
                                if (keepEdit == 0)
                                    return EditReturnListUrl.IsEmpty() ? (ActionResult)RedirectToAction("Index") : Redirect(EditReturnListUrl);
                                return RedirectToAction("Edit", new { id = _editId });
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(keyFieldName, MvcLogger.GetErrorMessage(ex));
                        }
                        ViewBag.EditMode = true;
                        ViewBag.EditAction = EditAction;
                        ViewBag.Title = GetTitle(TitleType.Editor);
                        BuildEditMenu();
                        ViewBag.PageMenu = PageMenu;
                        OnEditItem(org);
                        return View(PopupEditor ? ViewEditPopup : ViewEdit, org);
                    }
                    return new ForbidResult();
                }
            }
            return NotFound();
        }
        [HttpPost]
        public virtual async Task<JsonResult> EditPopup(T item)
        {
            var r = new JsonResultViewModel();
            if (!OnEditAuthCheck())
            {
                r.Error = "FORBIDDEN";
                return Json(r);
            }

            var keyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            var v = Digger.GetObjectValue(item, keyFieldName);
            if (v != null)
            {
                int.TryParse(v.ToString(), out _editId);
                if (_editId > 0)
                {
                    var org = await Repo.GetItemAsync(_editId);
                    await TryUpdateModelAsync(org);
                    if (OnEditSaveCheck(org))
                    {
                        OnEditSaveItem(org);
                        if (await Repo.EditAsync(org))
                        {
                            r.Ok = true;
                            if (!EditJsAction.IsEmpty())
                                r.JsFunction = EditJsAction;
                        }
                        else
                            r.Error = MvcLogger.GetErrorMessage(ModelState);
                    }
                    r.Error = "ERR";
                }
            }
            else
                r.Error = "NOT FOUND";
            return Json(r);
        }
        #endregion
    }
}
