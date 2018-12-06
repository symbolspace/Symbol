/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {

    /// <summary>
    /// 云Action信息。
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Attribute.DisplayName},{Url}")]
    public class CloudActionInfo : ICloudActionInfoSetter {

        #region fields
        private CloudControllerInfo _controller;
        private string _name;
        private string _url;
        private System.Reflection.MethodInfo _methodInfo;
        private IParameterInfo _returnParameter;
        private ParameterInfoList _parameters;
        private ParameterInfoList _args= new ParameterInfoList(true,false);
        private CloudActionAttribute _attribute;
        private CloudActionPermissonAttribute _permissonAttribute;
        private ProtocolSetting _protocol;
        private CloudActionCodeAttribute[] _codes;
        private CloudEventAttribute _eventAttribute;
        private Symbol.Routing.Route _route;
        private Symbol.Collections.Generic.NameValueCollection<object> _routeValues;
        private CloudActionFilterList _list_filter = new CloudActionFilterList();
        private bool _obsoleted;
        #endregion

        #region properties
        /// <summary>
        /// 获取Action是否已过时。
        /// </summary>
        public bool Obsoleted { get { return _obsoleted; } }
        /// <summary>
        /// 获取所在控制器的信息。
        /// </summary>
        public CloudControllerInfo Controller { get { return _controller; } }
        /// <summary>
        /// 获取Action的名称。
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// 获取Action调用地址。
        /// </summary>
        public string Url { get { return _url; } }
        /// <summary>
        /// 获取是否包含路由规则。
        /// </summary>
        public bool HasRoute { get { return _route != null; } }
        /// <summary>
        /// 获取路由对象。
        /// </summary>
        public Symbol.Routing.Route Route { get { return _route; } }
        /// <summary>
        /// 获取路由参数（仅限于路由匹配出来的对象）。
        /// </summary>
        public Symbol.Collections.Generic.NameValueCollection<object> RouteValues { get { return _routeValues; } }
        /// <summary>
        /// 获取Action对应的MethodInfo。
        /// </summary>
        public System.Reflection.MethodInfo MethodInfo { get { return _methodInfo; } }
        /// <summary>
        /// 获取返回值参数信息。
        /// </summary>
        public IParameterInfo ReturnParameter { get { return _returnParameter; } }
        /// <summary>
        /// 获取参数列表信息。
        /// </summary>
        public ParameterInfoList Parameters { get { return _parameters; } }
        /// <summary>
        /// 获取额外参数列表。
        /// </summary>
        public ParameterInfoList Args { get { return _args; } }
        /// <summary>
        /// 获取Action的特性信息。
        /// </summary>
        public CloudActionAttribute Attribute { get { return _attribute; } }
        /// <summary>
        /// 获取Event的特性信息。
        /// </summary>
        public CloudEventAttribute EventAttribute { get { return _eventAttribute; } }
        /// <summary>
        /// 获取Action的权限特性信息。
        /// </summary>
        public CloudActionPermissonAttribute PermissonAttribute { get { return _permissonAttribute; } }
        /// <summary>
        /// 获取协议配置。
        /// </summary>
        public ProtocolSetting Protocol { get { return _protocol; } }

        /// <summary>
        /// 获取Action的返回代码列表。
        /// </summary>
        public CloudActionCodeAttribute[] Codes { get { return _codes; } }
        /// <summary>
        /// 获取过滤器列表。
        /// </summary>
        public CloudActionFilterList Filters { get { return _list_filter; } }

        #endregion

        #region ICloudActionInfoSetter 成员

        CloudControllerInfo ICloudActionInfoSetter.Controller { set { _controller = value; } }
        string ICloudActionInfoSetter.Name { set { _name = value; } }
        string ICloudActionInfoSetter.Url {
            set {
                if (_url == value)
                    return;
                _url = value;
                _route = Symbol.Routing.Route.Parse(value);
            }
        }
        System.Reflection.MethodInfo ICloudActionInfoSetter.MethodInfo {
            set {
                _methodInfo = value;
                if (value == null) {
                    _obsoleted = _controller == null ? false : _controller.Obsoleted;
                } else {
                    if (_controller != null && _controller.Obsoleted)
                        _obsoleted = true;
                    else
                        _obsoleted = AttributeExtensions.IsDefined<ObsoleteAttribute>(value);
                }
            }
        }
        IParameterInfo ICloudActionInfoSetter.ReturnParameter { set { _returnParameter = value; } }
        ParameterInfoList ICloudActionInfoSetter.Parameters { set { _parameters = value; } }
        CloudActionAttribute ICloudActionInfoSetter.Attribute { set { _attribute = value; } }
        CloudEventAttribute ICloudActionInfoSetter.EventAttribute { set { _eventAttribute = value; } }
        CloudActionPermissonAttribute ICloudActionInfoSetter.PermissonAttribute { set { _permissonAttribute = value; } }

        ProtocolSetting ICloudActionInfoSetter.Protocol { set { _protocol = value; } }

        CloudActionCodeAttribute[] ICloudActionInfoSetter.Codes { set { _codes = value; } }
        Symbol.Collections.Generic.NameValueCollection<object> ICloudActionInfoSetter.RouteValues { set { _routeValues = value; } }

        #endregion

        #region Clone
        CloudActionInfo Clone() {
            CloudActionInfo info = new CloudActionInfo();
            info._obsoleted = _obsoleted;
            info._controller = _controller;
            info._url = _url;
            info._routeValues = _routeValues;
            info._name = _name;
            info._methodInfo = _methodInfo;
            info._returnParameter = _returnParameter;
            info._parameters = _parameters;
            info._args = _args;
            info._attribute = _attribute;
            info._permissonAttribute = _permissonAttribute;
            info._protocol = _protocol;
            info._codes = _codes;
            info._eventAttribute = _eventAttribute;
            info._list_filter = _list_filter;
            return info;
        }
        #endregion
        #region Match
        /// <summary>
        /// 尝试动态匹配。
        /// </summary>
        /// <param name="url">null或empty，返回null。</param>
        /// <returns>匹配不成功返回null。</returns>
        public CloudActionInfo Match(string url) {
            if (_route == null)
                return null;
            var values = _route.Match(url);
            if (values == null)
                return null;
            var info = Clone();
            info._url = url;
            info._routeValues = values;
            return info;
        }
        #endregion
        #region MatchEvent
        /// <summary>
        /// 尝试动态匹配事件。
        /// </summary>
        /// <param name="url">null或empty，返回null。</param>
        /// <returns>匹配不成功返回null。</returns>
        public CloudActionInfo MatchEvent(string url) {
            if (_eventAttribute == null || !_eventAttribute.HasRoute)
                return null;
            var values = _eventAttribute.Route.Match(url);
            if (values == null)
                return null;
            var info = Clone();
            info._routeValues = values;

            return info;
        }
        #endregion

    }
    /// <summary>
    /// 云Action信息(设置器)。
    /// </summary>
    interface ICloudActionInfoSetter {
        /// <summary>
        /// 设置所在控制器的信息。
        /// </summary>
        CloudControllerInfo Controller { set; }
        /// <summary>
        /// 设置Action的名称。
        /// </summary>
        string Name { set; }
        /// <summary>
        /// 设置Action调用地址。
        /// </summary>
        string Url { set; }
        /// <summary>
        /// 设置Action对应的MethodInfo。
        /// </summary>
        System.Reflection.MethodInfo MethodInfo { set; }
        /// <summary>
        /// 设置返回值参数信息。
        /// </summary>
        IParameterInfo ReturnParameter { set; }
        /// <summary>
        /// 设置参数列表信息。
        /// </summary>
        ParameterInfoList Parameters { set; }
        /// <summary>
        /// 设置Action的特性信息。
        /// </summary>
        CloudActionAttribute Attribute { set; }
        /// <summary>
        /// 设置Event的特性信息。
        /// </summary>
        CloudEventAttribute EventAttribute { set;}
        /// <summary>
        /// 设置Action的权限特性信息。
        /// </summary>
        CloudActionPermissonAttribute PermissonAttribute { set; }
        /// <summary>
        /// 设置协议配置。
        /// </summary>
        ProtocolSetting Protocol { set; }

        /// <summary>
        /// 设置Action的返回代码列表。
        /// </summary>
        CloudActionCodeAttribute[] Codes { set; }

        /// <summary>
        /// 设置路由参数（仅限于路由匹配出来的对象）。
        /// </summary>
        Symbol.Collections.Generic.NameValueCollection<object> RouteValues { set; }

    }
}
