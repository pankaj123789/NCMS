ko.components.register('item-menu', {
	viewModel: function (menu) {
		this.hasChildren = menu.Children && menu.Children.length > 0;
		this.css = this.hasChildren ? 'treeview' : '';
		this.menu = menu;
		this.menu.Icon = this.menu.Icon || "fa fa-circle-o";
	},
	template: '\
<li>\
    <a data-bind="css: css, attr: { href: menu.Route }">\
        <i data-bind="attr: { class: menu.Icon }"></i> <span data-bind="text: menu.Text"></span>\
		<!-- ko if: hasChildren -->\
		<i class="fa fa-angle-left pull-right"></i>\
		<!-- /ko -->\
    </a>\
	<!-- ko if: hasChildren -->\
	<ul class="treeview-menu">\
        <!-- ko foreach: { data: menu.Children, as: "child" } -->\
        <!-- ko component: { name: "item-menu", params: child } --><!-- /ko -->\
		<!-- /ko -->\
    </ul>\
	<!-- /ko -->\
</li>'
});