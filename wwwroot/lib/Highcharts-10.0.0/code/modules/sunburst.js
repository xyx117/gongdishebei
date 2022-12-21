/*
 Highcharts JS v10.0.0 (2022-03-07)

 (c) 2016-2021 Highsoft AS
 Authors: Jon Arild Nygard

 License: www.highcharts.com/license
*/
(function(a){"object"===typeof module&&module.exports?(a["default"]=a,module.exports=a):"function"===typeof define&&define.amd?define("highcharts/modules/sunburst",["highcharts"],function(w){a(w);a.Highcharts=w;return a}):a("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(a){function w(a,l,d,r){a.hasOwnProperty(l)||(a[l]=r.apply(null,d),"function"===typeof CustomEvent&&window.dispatchEvent(new CustomEvent("HighchartsModuleLoaded",{detail:{path:l,module:a[l]}})))}a=a?a._modules:{};w(a,
"Series/ColorMapMixin.js",[a["Core/Globals.js"],a["Core/Series/Point.js"],a["Core/Utilities.js"]],function(a,l,d){var r=a.noop;a=a.seriesTypes;var k=d.defined;d=d.addEvent;d(l,"afterSetState",function(a){this.moveToTopOnHover&&this.graphic&&this.graphic.attr({zIndex:a&&"hover"===a.state?1:0})});return{PointMixin:{dataLabelOnNull:!0,moveToTopOnHover:!0,isValid:function(){return null!==this.value&&Infinity!==this.value&&-Infinity!==this.value}},SeriesMixin:{pointArrayMap:["value"],axisTypes:["xAxis",
"yAxis","colorAxis"],trackerGroups:["group","markerGroup","dataLabelsGroup"],getSymbol:r,parallelArrays:["x","y","value"],colorKey:"value",pointAttribs:a.column.prototype.pointAttribs,colorAttribs:function(a){var t={};!k(a.color)||a.state&&"normal"!==a.state||(t[this.colorProp||"fill"]=a.color);return t}}}});w(a,"Series/Treemap/TreemapAlgorithmGroup.js",[],function(){return function(){function a(a,d,r,k){this.height=a;this.width=d;this.plot=k;this.startDirection=this.direction=r;this.lH=this.nH=this.lW=
this.nW=this.total=0;this.elArr=[];this.lP={total:0,lH:0,nH:0,lW:0,nW:0,nR:0,lR:0,aspectRatio:function(a,e){return Math.max(a/e,e/a)}}}a.prototype.addElement=function(a){this.lP.total=this.elArr[this.elArr.length-1];this.total+=a;0===this.direction?(this.lW=this.nW,this.lP.lH=this.lP.total/this.lW,this.lP.lR=this.lP.aspectRatio(this.lW,this.lP.lH),this.nW=this.total/this.height,this.lP.nH=this.lP.total/this.nW,this.lP.nR=this.lP.aspectRatio(this.nW,this.lP.nH)):(this.lH=this.nH,this.lP.lW=this.lP.total/
this.lH,this.lP.lR=this.lP.aspectRatio(this.lP.lW,this.lH),this.nH=this.total/this.width,this.lP.nW=this.lP.total/this.nH,this.lP.nR=this.lP.aspectRatio(this.lP.nW,this.nH));this.elArr.push(a)};a.prototype.reset=function(){this.lW=this.nW=0;this.elArr=[];this.total=0};return a}()});w(a,"Series/DrawPointComposition.js",[],function(){var a;(function(a){function d(a){var e=this,g=a.animatableAttribs,d=a.onComplete,c=a.css,b=a.renderer,A=this.series&&this.series.chart.hasRendered?void 0:this.series&&
this.series.options.animation,y=this.graphic;a.attribs=a.attribs||{};a.attribs["class"]=this.getClassName();if(this.shouldDraw())y||(this.graphic=y="text"===a.shapeType?b.text():b[a.shapeType](a.shapeArgs||{}),y.add(a.group)),c&&y.css(c),y.attr(a.attribs).animate(g,a.isNew?!1:A,d);else if(y){var m=function(){e.graphic=y=y&&y.destroy();"function"===typeof d&&d()};Object.keys(g).length?y.animate(g,void 0,function(){m()}):m()}}function l(){return!this.isNull}var k=[];a.compose=function(a){if(-1===k.indexOf(a)){k.push(a);
var e=a.prototype;e.draw=d;e.shouldDraw||(e.shouldDraw=l)}return a}})(a||(a={}));return a});w(a,"Series/Treemap/TreemapPoint.js",[a["Series/DrawPointComposition.js"],a["Core/Series/SeriesRegistry.js"],a["Core/Utilities.js"]],function(a,l,d){var r=this&&this.__extends||function(){var a=function(c,A){a=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(a,c){a.__proto__=c}||function(a,c){for(var b in c)c.hasOwnProperty(b)&&(a[b]=c[b])};return a(c,A)};return function(c,A){function b(){this.constructor=
c}a(c,A);c.prototype=null===A?Object.create(A):(b.prototype=A.prototype,new b)}}(),k=l.series.prototype.pointClass,t=l.seriesTypes;l=t.pie.prototype.pointClass;var e=d.extend,g=d.isNumber,v=d.pick;d=function(a){function c(){var c=null!==a&&a.apply(this,arguments)||this;c.name=void 0;c.node=void 0;c.options=void 0;c.series=void 0;c.value=void 0;return c}r(c,a);c.prototype.getClassName=function(){var a=k.prototype.getClassName.call(this),c=this.series,b=c.options;this.node.level<=c.nodeMap[c.rootNode].level?
a+=" highcharts-above-level":this.node.isLeaf||v(b.interactByLeaf,!b.allowTraversingTree)?this.node.isLeaf||(a+=" highcharts-internal-node"):a+=" highcharts-internal-node-interactive";return a};c.prototype.isValid=function(){return!(!this.id&&!g(this.value))};c.prototype.setState=function(a){k.prototype.setState.call(this,a);this.graphic&&this.graphic.attr({zIndex:"hover"===a?1:0})};c.prototype.shouldDraw=function(){return g(this.plotY)&&null!==this.y};return c}(t.scatter.prototype.pointClass);e(d.prototype,
{setVisible:l.prototype.setVisible});a.compose(d);return d});w(a,"Series/Treemap/TreemapUtilities.js",[a["Core/Utilities.js"]],function(a){var l=a.objectEach,d;(function(a){function d(a,e,g){void 0===g&&(g=this);a=e.call(g,a);!1!==a&&d(a,e,g)}a.AXIS_MAX=100;a.isBoolean=function(a){return"boolean"===typeof a};a.eachObject=function(a,d,g){g=g||this;l(a,function(e,c){d.call(g,e,c,a)})};a.recursive=d})(d||(d={}));return d});w(a,"Series/TreeUtilities.js",[a["Core/Color/Color.js"],a["Core/Utilities.js"]],
function(a,l){function d(a,b){var c=b.before,e=b.idRoot,m=b.mapIdToNode[e],u=b.points[a.i],x=u&&u.options||{},C=[],p=0;a.levelDynamic=a.level-(!1!==b.levelIsConstant?0:m.level);a.name=v(u&&u.name,"");a.visible=e===a.id||!0===b.visible;"function"===typeof c&&(a=c(a,b));a.children.forEach(function(c,u){var x=r({},b);r(x,{index:u,siblings:a.children.length,visible:a.visible});c=d(c,x);C.push(c);c.visible&&(p+=c.val)});c=v(x.value,p);a.visible=0<=c&&(0<p||a.visible);a.children=C;a.childrenTotal=p;a.isLeaf=
a.visible&&!p;a.val=c;return a}var r=l.extend,k=l.isArray,t=l.isNumber,e=l.isObject,g=l.merge,v=l.pick;return{getColor:function(c,b){var d=b.index,e=b.mapOptionsToLevel,m=b.parentColor,u=b.parentColorIndex,x=b.series,C=b.colors,p=b.siblings,g=x.points,l=x.chart.options.chart,k;if(c){g=g[c.i];c=e[c.level]||{};if(e=g&&c.colorByPoint){var r=g.index%(C?C.length:l.colorCount);var t=C&&C[r]}if(!x.chart.styledMode){C=g&&g.options.color;l=c&&c.color;if(k=m)k=(k=c&&c.colorVariation)&&"brightness"===k.key&&
d&&p?a.parse(m).brighten(d/p*k.to).get():m;k=v(C,l,t,k,x.color)}var K=v(g&&g.options.colorIndex,c&&c.colorIndex,r,u,b.colorIndex)}return{color:k,colorIndex:K}},getLevelOptions:function(a){var c=null;if(e(a)){c={};var d=t(a.from)?a.from:1;var l=a.levels;var m={};var u=e(a.defaults)?a.defaults:{};k(l)&&(m=l.reduce(function(a,c){if(e(c)&&t(c.level)){var b=g({},c);var m=v(b.levelIsConstant,u.levelIsConstant);delete b.levelIsConstant;delete b.level;c=c.level+(m?0:d-1);e(a[c])?g(!0,a[c],b):a[c]=b}return a},
{}));l=t(a.to)?a.to:1;for(a=0;a<=l;a++)c[a]=g({},u,e(m[a])?m[a]:{})}return c},setTreeValues:d,updateRootId:function(a){if(e(a)){var c=e(a.options)?a.options:{};c=v(a.rootNode,c.rootId,"");e(a.userOptions)&&(a.userOptions.rootId=c);a.rootNode=c}return c}}});w(a,"Extensions/Breadcrumbs.js",[a["Core/Chart/Chart.js"],a["Core/Globals.js"],a["Core/DefaultOptions.js"],a["Core/Utilities.js"],a["Core/FormatUtilities.js"]],function(a,l,d,r,k){var t=k.format;k=r.addEvent;var e=r.objectEach,g=r.extend,v=r.fireEvent,
c=r.merge,b=r.pick,A=r.defined,y=r.isString;g(d.defaultOptions.lang,{mainBreadcrumb:"Main"});d=function(){function a(b,x){this.group=void 0;this.list=[];this.elementList={};this.isDirty=!0;this.level=0;this.options=void 0;x=c(b.options.drilldown&&b.options.drilldown.drillUpButton,a.defaultBreadcrumbsOptions,b.options.navigation&&b.options.navigation.breadcrumbs,x);this.chart=b;this.options=x||{}}a.prototype.updateProperties=function(a){this.setList(a);this.setLevel();this.isDirty=!0};a.prototype.setList=
function(a){this.list=a};a.prototype.setLevel=function(){this.level=this.list.length&&this.list.length-1};a.prototype.getLevel=function(){return this.level};a.prototype.getButtonText=function(a){var c=this.chart,d=this.options,p=c.options.lang,u=b(d.format,d.showFullPath?"{level.name}":"\u2190 {level.name}");p=p&&b(p.drillUpText,p.mainBreadcrumb);a=d.formatter&&d.formatter(a)||t(u,{level:a.levelOptions},c)||"";(y(a)&&!a.length||"\u2190 "===a)&&A(p)&&(a=d.showFullPath?p:"\u2190 "+p);return a};a.prototype.redraw=
function(){this.isDirty&&this.render();this.group&&this.group.align();this.isDirty=!1};a.prototype.render=function(){var a=this.chart,c=this.options;!this.group&&c&&(this.group=a.renderer.g("breadcrumbs-group").addClass("highcharts-no-tooltip highcharts-breadcrumbs").attr({zIndex:c.zIndex}).add());c.showFullPath?this.renderFullPathButtons():this.renderSingleButton();this.alignBreadcrumbsGroup()};a.prototype.renderFullPathButtons=function(){this.destroySingleButton();this.resetElementListState();this.updateListElements();
this.destroyListElements()};a.prototype.renderSingleButton=function(){var a=this.chart,c=this.list,b=this.options.buttonSpacing;this.destroyListElements();var d=this.group?this.group.getBBox().width:b;c=c[c.length-2];!a.drillUpButton&&0<this.level?a.drillUpButton=this.renderButton(c,d,b):a.drillUpButton&&(0<this.level?this.updateSingleButton():this.destroySingleButton())};a.prototype.alignBreadcrumbsGroup=function(a){if(this.group){var d=this.options,g=d.buttonTheme,p=d.position,e="chart"===d.relativeTo||
"spacingBox"===d.relativeTo?void 0:"scrollablePlotBox",u=this.group.getBBox();d=2*(g.padding||0)+d.buttonSpacing;p.width=u.width+d;p.height=u.height+d;p=c(p);a&&(p.x+=a);p.y=b(p.y,this.yOffset,0);this.group.align(p,!0,e)}};a.prototype.renderButton=function(a,b,d){var p=this,g=this.chart,e=p.options,u=c(e.buttonTheme),m=u.states;delete u.states;b=g.renderer.button(p.getButtonText(a),b,d,function(c){var b=e.events&&e.events.click,d;b&&(d=b.call(p,c,a));!1!==d&&(c.newLevel=e.showFullPath?a.level:p.level-
1,v(p,"up",c))},u,m&&m.hover,m&&m.select,m&&m.disabled).addClass("highcharts-breadcrumbs-button").add(p.group);g.styledMode||b.attr(e.style);return b};a.prototype.renderSeparator=function(a,c){var b=this.chart,d=this.options.separator;a=b.renderer.label(d.text,a,c,void 0,void 0,void 0,!1).addClass("highcharts-breadcrumbs-separator").add(this.group);b.styledMode||a.css(d.style);return a};a.prototype.update=function(a){c(!0,this.options,a);this.destroy();this.isDirty=!0};a.prototype.updateSingleButton=
function(){var a=this.chart,c=this.list[this.level-1];a.drillUpButton&&a.drillUpButton.attr({text:this.getButtonText(c)})};a.prototype.destroy=function(){this.destroySingleButton();this.destroyListElements(!0);this.group&&this.group.destroy();this.group=void 0};a.prototype.destroyListElements=function(a){var c=this.elementList;e(c,function(b,d){if(a||!c[d].updated)b=c[d],b.button&&b.button.destroy(),b.separator&&b.separator.destroy(),delete b.button,delete b.separator,delete c[d]});a&&(this.elementList=
{})};a.prototype.destroySingleButton=function(){this.chart.drillUpButton&&(this.chart.drillUpButton.destroy(),this.chart.drillUpButton=void 0)};a.prototype.resetElementListState=function(){e(this.elementList,function(a){a.updated=!1})};a.prototype.updateListElements=function(){var a=function(a,c){return a.getBBox().width+c},c=this,b=c.elementList,d=c.options.buttonSpacing,e=c.list,g=c.group?a(c.group,d):d,m;e.forEach(function(l,p){p=p===e.length-1;if(b[l.level]){m=b[l.level];var k=m.button;m.separator||
p?m.separator&&p&&(m.separator.destroy(),delete m.separator):(g+=d,m.separator=c.renderSeparator(g,d),g+=a(m.separator,d));b[l.level].updated=!0}else{k=c.renderButton(l,g,d);g+=a(k,d);if(!p){var v=c.renderSeparator(g,d);g+=a(v,d)}b[l.level]={button:k,separator:v,updated:!0}}k&&k.setState(p?2:0)})};a.defaultBreadcrumbsOptions={buttonTheme:{fill:"none",height:18,padding:2,"stroke-width":0,zIndex:7,states:{select:{fill:"none"}},style:{color:"#335cad"}},buttonSpacing:5,floating:!1,format:void 0,relativeTo:"plotBox",
position:{align:"left",verticalAlign:"top",x:0,y:void 0},separator:{text:"/",style:{color:"#666666"}},showFullPath:!0,style:{},useHTML:!1,zIndex:7};return a}();l.Breadcrumbs||(l.Breadcrumbs=d,k(a,"getMargins",function(){var a=this.breadcrumbs;if(a&&!a.options.floating&&a.level){var c=a.options,b=c.buttonTheme;b=(b.height||0)+2*(b.padding||0)+c.buttonSpacing;c=c.position.verticalAlign;"bottom"===c?(this.marginBottom=(this.marginBottom||0)+b,a.yOffset=b):"middle"!==c?(this.plotTop+=b,a.yOffset=-b):
a.yOffset=void 0}}),k(a,"redraw",function(){this.breadcrumbs&&this.breadcrumbs.redraw()}),k(a,"destroy",function(){this.breadcrumbs&&(this.breadcrumbs.destroy(),this.breadcrumbs=void 0)}),k(a,"afterShowResetZoom",function(){if(this.breadcrumbs){var a=this.resetZoomButton&&this.resetZoomButton.getBBox(),c=this.breadcrumbs.options;a&&"right"===c.position.align&&"plotBox"===c.relativeTo&&this.breadcrumbs.alignBreadcrumbsGroup(-a.width-c.buttonSpacing)}}),k(a,"selection",function(a){!0===a.resetSelection&&
this.breadcrumbs&&this.breadcrumbs.alignBreadcrumbsGroup()}));"";return d});w(a,"Series/Treemap/TreemapComposition.js",[a["Core/Series/SeriesRegistry.js"],a["Series/Treemap/TreemapUtilities.js"],a["Core/Utilities.js"]],function(a,l,d){var r=d.addEvent,k=d.extend,t=!1;r(a.series,"afterBindAxes",function(){var a=this.xAxis,d=this.yAxis;if(a&&d)if(this.is("treemap")){var v={endOnTick:!1,gridLineWidth:0,lineWidth:0,min:0,minPadding:0,max:l.AXIS_MAX,maxPadding:0,startOnTick:!1,title:void 0,tickPositions:[]};
k(d.options,v);k(a.options,v);t=!0}else t&&(d.setOptions(d.userOptions),a.setOptions(a.userOptions),t=!1)})});w(a,"Series/Treemap/TreemapSeries.js",[a["Core/Color/Color.js"],a["Series/ColorMapMixin.js"],a["Core/Globals.js"],a["Core/Legend/LegendSymbol.js"],a["Core/Series/SeriesRegistry.js"],a["Series/Treemap/TreemapAlgorithmGroup.js"],a["Series/Treemap/TreemapPoint.js"],a["Series/Treemap/TreemapUtilities.js"],a["Series/TreeUtilities.js"],a["Extensions/Breadcrumbs.js"],a["Core/Utilities.js"]],function(a,
l,d,r,k,t,e,g,v,c,b){var A=this&&this.__extends||function(){var a=function(c,f){a=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(a,f){a.__proto__=f}||function(a,f){for(var c in f)f.hasOwnProperty(c)&&(a[c]=f[c])};return a(c,f)};return function(c,f){function n(){this.constructor=c}a(c,f);c.prototype=null===f?Object.create(f):(n.prototype=f.prototype,new n)}}(),y=a.parse;a=d.noop;var m=k.series;d=k.seriesTypes;var u=d.column,x=d.heatmap,w=d.scatter,p=v.getColor,R=v.getLevelOptions,
K=v.updateRootId,D=b.addEvent,H=b.correctFloat,G=b.defined,I=b.error,J=b.extend,N=b.fireEvent,O=b.isArray,T=b.isObject,h=b.isString,E=b.merge,B=b.pick,U=b.stableSort;v=function(a){function b(){var f=null!==a&&a.apply(this,arguments)||this;f.axisRatio=void 0;f.data=void 0;f.mapOptionsToLevel=void 0;f.nodeMap=void 0;f.options=void 0;f.points=void 0;f.rootNode=void 0;f.tree=void 0;f.level=void 0;return f}A(b,a);b.prototype.algorithmCalcPoints=function(a,c,b,d){var f,n,q,h,M=b.lW,g=b.lH,e=b.plot,l=0,
p=b.elArr.length-1;if(c)M=b.nW,g=b.nH;else var m=b.elArr[b.elArr.length-1];b.elArr.forEach(function(a){if(c||l<p)0===b.direction?(f=e.x,n=e.y,q=M,h=a/q):(f=e.x,n=e.y,h=g,q=a/h),d.push({x:f,y:n,width:q,height:H(h)}),0===b.direction?e.y+=h:e.x+=q;l+=1});b.reset();0===b.direction?b.width-=M:b.height-=g;e.y=e.parent.y+(e.parent.height-b.height);e.x=e.parent.x+(e.parent.width-b.width);a&&(b.direction=1-b.direction);c||b.addElement(m)};b.prototype.algorithmFill=function(a,b,c){var f=[],n,d=b.direction,
q=b.x,h=b.y,e=b.width,g=b.height,l,p,m,k;c.forEach(function(c){n=c.val/b.val*b.height*b.width;l=q;p=h;0===d?(k=g,m=n/k,e-=m,q+=m):(m=e,k=n/m,g-=k,h+=k);f.push({x:l,y:p,width:m,height:k});a&&(d=1-d)});return f};b.prototype.algorithmLowAspectRatio=function(a,b,c){var f=[],n=this,d,q={x:b.x,y:b.y,parent:b},h=0,e=c.length-1,g=new t(b.height,b.width,b.direction,q);c.forEach(function(c){d=c.val/b.val*b.height*b.width;g.addElement(d);g.lP.nR>g.lP.lR&&n.algorithmCalcPoints(a,!1,g,f,q);h===e&&n.algorithmCalcPoints(a,
!0,g,f,q);h+=1});return f};b.prototype.alignDataLabel=function(a,b,c){var f=c.style;f&&!G(f.textOverflow)&&b.text&&b.getBBox().width>b.text.textWidth&&b.css({textOverflow:"ellipsis",width:f.width+="px"});u.prototype.alignDataLabel.apply(this,arguments);a.dataLabel&&a.dataLabel.attr({zIndex:(a.node.zIndex||0)+1})};b.prototype.buildNode=function(a,b,c,d,h){var f=this,n=[],q=f.points[b],F=0,e;(d[a]||[]).forEach(function(b){e=f.buildNode(f.points[b].id,b,c+1,d,a);F=Math.max(e.height+1,F);n.push(e)});
b={id:a,i:b,children:n,height:F,level:c,parent:h,visible:!1};f.nodeMap[b.id]=b;q&&(q.node=b);return b};b.prototype.calculateChildrenAreas=function(a,b){var f=this,c=f.options,d=f.mapOptionsToLevel[a.level+1],n=B(f[d&&d.layoutAlgorithm]&&d.layoutAlgorithm,c.layoutAlgorithm),h=c.alternateStartingDirection,e=[];a=a.children.filter(function(a){return!a.ignore});d&&d.layoutStartingDirection&&(b.direction="vertical"===d.layoutStartingDirection?0:1);e=f[n](b,a);a.forEach(function(a,c){c=e[c];a.values=E(c,
{val:a.childrenTotal,direction:h?1-b.direction:b.direction});a.pointValues=E(c,{x:c.x/f.axisRatio,y:g.AXIS_MAX-c.y-c.height,width:c.width/f.axisRatio});a.children.length&&f.calculateChildrenAreas(a,a.values)})};b.prototype.createList=function(a){var f=this.chart,b=[];if(f.breadcrumbs){var c=0;b.push({level:c,levelOptions:f.series[0]});f=a.target.nodeMap[a.newRootId];for(var d=[];f.parent||""===f.parent;)d.push(f),f=a.target.nodeMap[f.parent];d.reverse().forEach(function(a){b.push({level:++c,levelOptions:a})});
1>=b.length&&(b.length=0)}return b};b.prototype.drawDataLabels=function(){var a=this,b=a.mapOptionsToLevel,c,d;a.points.filter(function(a){return a.node.visible}).forEach(function(f){d=b[f.node.level];c={style:{}};f.node.isLeaf||(c.enabled=!1);d&&d.dataLabels&&(c=E(c,d.dataLabels),a._hasPointLabels=!0);f.shapeArgs&&(c.style.width=f.shapeArgs.width,f.dataLabel&&f.dataLabel.css({width:f.shapeArgs.width+"px"}));f.dlOptions=E(c,f.options.dataLabels)});m.prototype.drawDataLabels.call(this)};b.prototype.drawPoints=
function(){var a=this,b=a.chart,c=b.renderer,d=b.styledMode,h=a.options,e=d?{}:h.shadow,g=h.borderRadius,m=b.pointCount<h.animationLimit,l=h.allowTraversingTree;a.points.forEach(function(b){var f=b.node.levelDynamic,n={},q={},P={},F="level-group-"+b.node.level,Q=!!b.graphic,p=m&&Q,k=b.shapeArgs;b.shouldDraw()&&(b.isInside=!0,g&&(q.r=g),E(!0,p?n:q,Q?k:{},d?{}:a.pointAttribs(b,b.selected?"select":void 0)),a.colorAttribs&&d&&J(P,a.colorAttribs(b)),a[F]||(a[F]=c.g(F).attr({zIndex:1E3-(f||0)}).add(a.group),
a[F].survive=!0));b.draw({animatableAttribs:n,attribs:q,css:P,group:a[F],renderer:c,shadow:e,shapeArgs:k,shapeType:"rect"});l&&b.graphic&&(b.drillId=h.interactByLeaf?a.drillToByLeaf(b):a.drillToByGroup(b))})};b.prototype.drillToByGroup=function(a){var b=!1;1!==a.node.level-this.nodeMap[this.rootNode].level||a.node.isLeaf||(b=a.id);return b};b.prototype.drillToByLeaf=function(a){var b=!1;if(a.node.parent!==this.rootNode&&a.node.isLeaf)for(a=a.node;!b;)a=this.nodeMap[a.parent],a.parent===this.rootNode&&
(b=a.id);return b};b.prototype.drillToNode=function(a,b){I(32,!1,void 0,{"treemap.drillToNode":"use treemap.setRootNode"});this.setRootNode(a,b)};b.prototype.drillUp=function(){var a=this.nodeMap[this.rootNode];a&&h(a.parent)&&this.setRootNode(a.parent,!0,{trigger:"traverseUpButton"})};b.prototype.getExtremes=function(){var a=m.prototype.getExtremes.call(this,this.colorValueData),b=a.dataMax;this.valueMin=a.dataMin;this.valueMax=b;return m.prototype.getExtremes.call(this)};b.prototype.getListOfParents=
function(a,b){a=O(a)?a:[];var c=O(b)?b:[];b=a.reduce(function(a,b,c){b=B(b.parent,"");"undefined"===typeof a[b]&&(a[b]=[]);a[b].push(c);return a},{"":[]});g.eachObject(b,function(a,b,f){""!==b&&-1===c.indexOf(b)&&(a.forEach(function(a){f[""].push(a)}),delete f[b])});return b};b.prototype.getTree=function(){var a=this.data.map(function(a){return a.id});a=this.getListOfParents(this.data,a);this.nodeMap={};return this.buildNode("",-1,0,a)};b.prototype.hasData=function(){return!!this.processedXData.length};
b.prototype.init=function(a,b){var f=this,d=E(b.drillUpButton,b.breadcrumbs);this.colorAttribs=l.SeriesMixin.colorAttribs;var h=D(f,"setOptions",function(a){a=a.userOptions;G(a.allowDrillToNode)&&!G(a.allowTraversingTree)&&(a.allowTraversingTree=a.allowDrillToNode,delete a.allowDrillToNode);G(a.drillUpButton)&&!G(a.traverseUpButton)&&(a.traverseUpButton=a.drillUpButton,delete a.drillUpButton)});m.prototype.init.call(f,a,b);delete f.opacity;f.eventsToUnbind.push(h);f.options.allowTraversingTree&&(f.eventsToUnbind.push(D(f,
"click",f.onClickDrillToNode)),f.eventsToUnbind.push(D(f,"setRootNode",function(a){var b=f.chart;b.breadcrumbs&&b.breadcrumbs.updateProperties(f.createList(a))})),f.eventsToUnbind.push(D(f,"update",function(a,b){(b=this.chart.breadcrumbs)&&a.options.breadcrumbs&&b.update(a.options.breadcrumbs)})),f.eventsToUnbind.push(D(f,"destroy",function(a){var b=this.chart;b.breadcrumbs&&(b.breadcrumbs.destroy(),a.keepEventsForUpdate||(b.breadcrumbs=void 0))})));a.breadcrumbs||(a.breadcrumbs=new c(a,d));f.eventsToUnbind.push(D(a.breadcrumbs,
"up",function(a){a=this.level-a.newLevel;for(var b=0;b<a;b++)f.drillUp()}))};b.prototype.onClickDrillToNode=function(a){var b=(a=a.point)&&a.drillId;h(b)&&(a.setState(""),this.setRootNode(b,!0,{trigger:"click"}))};b.prototype.pointAttribs=function(a,b){var c=T(this.mapOptionsToLevel)?this.mapOptionsToLevel:{},f=a&&c[a.node.level]||{};c=this.options;var d=b&&c.states[b]||{},h=a&&a.getClassName()||"";a={stroke:a&&a.borderColor||f.borderColor||d.borderColor||c.borderColor,"stroke-width":B(a&&a.borderWidth,
f.borderWidth,d.borderWidth,c.borderWidth),dashstyle:a&&a.borderDashStyle||f.borderDashStyle||d.borderDashStyle||c.borderDashStyle,fill:a&&a.color||this.color};-1!==h.indexOf("highcharts-above-level")?(a.fill="none",a["stroke-width"]=0):-1!==h.indexOf("highcharts-internal-node-interactive")?(b=B(d.opacity,c.opacity),a.fill=y(a.fill).setOpacity(b).get(),a.cursor="pointer"):-1!==h.indexOf("highcharts-internal-node")?a.fill="none":b&&(a.fill=y(a.fill).brighten(d.brightness).get());return a};b.prototype.setColorRecursive=
function(a,b,c,d,h){var f=this,e=f&&f.chart;e=e&&e.options&&e.options.colors;if(a){var g=p(a,{colors:e,index:d,mapOptionsToLevel:f.mapOptionsToLevel,parentColor:b,parentColorIndex:c,series:f,siblings:h});if(b=f.points[a.i])b.color=g.color,b.colorIndex=g.colorIndex;(a.children||[]).forEach(function(b,c){f.setColorRecursive(b,g.color,g.colorIndex,c,a.children.length)})}};b.prototype.setPointValues=function(){var a=this,b=a.xAxis,c=a.yAxis,d=a.chart.styledMode;a.points.forEach(function(f){var h=f.node,
e=h.pointValues;h=h.visible;if(e&&h){h=e.height;var g=e.width,m=e.x,n=e.y,l=d?0:(a.pointAttribs(f)["stroke-width"]||0)%2/2;e=Math.round(b.toPixels(m,!0))-l;g=Math.round(b.toPixels(m+g,!0))-l;m=Math.round(c.toPixels(n,!0))-l;h=Math.round(c.toPixels(n+h,!0))-l;h={x:Math.min(e,g),y:Math.min(m,h),width:Math.abs(g-e),height:Math.abs(h-m)};f.plotX=h.x+h.width/2;f.plotY=h.y+h.height/2;f.shapeArgs=h}else delete f.plotX,delete f.plotY})};b.prototype.setRootNode=function(a,b,c){a=J({newRootId:a,previousRootId:this.rootNode,
redraw:B(b,!0),series:this},c);N(this,"setRootNode",a,function(a){var b=a.series;b.idPreviousRoot=a.previousRootId;b.rootNode=a.newRootId;b.isDirty=!0;a.redraw&&b.chart.redraw()})};b.prototype.setState=function(a){this.options.inactiveOtherPoints=!0;m.prototype.setState.call(this,a,!1);this.options.inactiveOtherPoints=!1};b.prototype.setTreeValues=function(a){var b=this,c=b.options,f=b.nodeMap[b.rootNode];c=g.isBoolean(c.levelIsConstant)?c.levelIsConstant:!0;var d=0,h=[],e=b.points[a.i];a.children.forEach(function(a){a=
b.setTreeValues(a);h.push(a);a.ignore||(d+=a.val)});U(h,function(a,b){return(a.sortIndex||0)-(b.sortIndex||0)});var m=B(e&&e.options.value,d);e&&(e.value=m);J(a,{children:h,childrenTotal:d,ignore:!(B(e&&e.visible,!0)&&0<m),isLeaf:a.visible&&!d,levelDynamic:a.level-(c?0:f.level),name:B(e&&e.name,""),sortIndex:B(e&&e.sortIndex,-m),val:m});return a};b.prototype.sliceAndDice=function(a,b){return this.algorithmFill(!0,a,b)};b.prototype.squarified=function(a,b){return this.algorithmLowAspectRatio(!0,a,
b)};b.prototype.strip=function(a,b){return this.algorithmLowAspectRatio(!1,a,b)};b.prototype.stripes=function(a,b){return this.algorithmFill(!1,a,b)};b.prototype.translate=function(){var a=this,b=a.options,c=K(a);m.prototype.translate.call(a);var d=a.tree=a.getTree();var h=a.nodeMap[c];""===c||h&&h.children.length||(a.setRootNode("",!1),c=a.rootNode,h=a.nodeMap[c]);a.mapOptionsToLevel=R({from:h.level+1,levels:b.levels,to:d.height,defaults:{levelIsConstant:a.options.levelIsConstant,colorByPoint:b.colorByPoint}});
g.recursive(a.nodeMap[a.rootNode],function(b){var c=!1,d=b.parent;b.visible=!0;if(d||""===d)c=a.nodeMap[d];return c});g.recursive(a.nodeMap[a.rootNode].children,function(a){var b=!1;a.forEach(function(a){a.visible=!0;a.children.length&&(b=(b||[]).concat(a.children))});return b});a.setTreeValues(d);a.axisRatio=a.xAxis.len/a.yAxis.len;a.nodeMap[""].pointValues=c={x:0,y:0,width:g.AXIS_MAX,height:g.AXIS_MAX};a.nodeMap[""].values=c=E(c,{width:c.width*a.axisRatio,direction:"vertical"===b.layoutStartingDirection?
0:1,val:d.val});a.calculateChildrenAreas(d,c);a.colorAxis||b.colorByPoint||a.setColorRecursive(a.tree);b.allowTraversingTree&&(b=h.pointValues,a.xAxis.setExtremes(b.x,b.x+b.width,!1),a.yAxis.setExtremes(b.y,b.y+b.height,!1),a.xAxis.setScale(),a.yAxis.setScale());a.setPointValues()};b.defaultOptions=E(w.defaultOptions,{allowTraversingTree:!1,animationLimit:250,borderRadius:0,showInLegend:!1,marker:void 0,colorByPoint:!1,dataLabels:{defer:!1,enabled:!0,formatter:function(){var a=this&&this.point?this.point:
{};return h(a.name)?a.name:""},inside:!0,verticalAlign:"middle"},tooltip:{headerFormat:"",pointFormat:"<b>{point.name}</b>: {point.value}<br/>"},ignoreHiddenPoint:!0,layoutAlgorithm:"sliceAndDice",layoutStartingDirection:"vertical",alternateStartingDirection:!1,levelIsConstant:!0,traverseUpButton:{position:{align:"right",x:-10,y:10}},borderColor:"#e6e6e6",borderWidth:1,colorKey:"colorValue",opacity:.15,states:{hover:{borderColor:"#999999",brightness:x?0:.1,halo:!1,opacity:.75,shadow:!1}}});return b}(w);
J(v.prototype,{buildKDTree:a,colorKey:"colorValue",directTouch:!0,drawLegendSymbol:r.drawRectangle,getExtremesFromAll:!0,getSymbol:a,optionalAxis:"colorAxis",parallelArrays:["x","y","value","colorValue"],pointArrayMap:["value"],pointClass:e,trackerGroups:["group","dataLabelsGroup"],utils:{recursive:g.recursive}});k.registerSeriesType("treemap",v);"";return v});w(a,"Series/Sunburst/SunburstPoint.js",[a["Series/DrawPointComposition.js"],a["Core/Series/SeriesRegistry.js"],a["Core/Utilities.js"]],function(a,
l,d){var r=this&&this.__extends||function(){var a=function(d,e){a=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(a,b){a.__proto__=b}||function(a,b){for(var c in b)b.hasOwnProperty(c)&&(a[c]=b[c])};return a(d,e)};return function(d,e){function c(){this.constructor=d}a(d,e);d.prototype=null===e?Object.create(e):(c.prototype=e.prototype,new c)}}(),k=l.series.prototype.pointClass,t=d.correctFloat;d=d.extend;l=function(a){function d(){var d=null!==a&&a.apply(this,arguments)||this;d.node=
void 0;d.options=void 0;d.series=void 0;d.shapeExisting=void 0;return d}r(d,a);d.prototype.getDataLabelPath=function(a){var c=this.series.chart.renderer,b=this.shapeExisting,d=b.start,e=b.end,g=d+(e-d)/2;g=0>g&&g>-Math.PI||g>Math.PI;var l=b.r+(a.options.distance||0);d===-Math.PI/2&&t(e)===t(1.5*Math.PI)&&(d=-Math.PI+Math.PI/360,e=-Math.PI/360,g=!0);if(e-d>Math.PI){g=!1;var k=!0}this.dataLabelPath&&(this.dataLabelPath=this.dataLabelPath.destroy());this.dataLabelPath=c.arc({open:!0,longArc:k?1:0}).add(a);
this.dataLabelPath.attr({start:g?d:e,end:g?e:d,clockwise:+g,x:b.x,y:b.y,r:(l+b.innerR)/2});return this.dataLabelPath};d.prototype.isValid=function(){return!0};return d}(l.seriesTypes.treemap.prototype.pointClass);d(l.prototype,{getClassName:k.prototype.getClassName,haloPath:k.prototype.haloPath,setState:k.prototype.setState});a.compose(l);return l});w(a,"Series/Sunburst/SunburstUtilities.js",[a["Core/Series/SeriesRegistry.js"],a["Core/Utilities.js"]],function(a,l){var d=a.seriesTypes.treemap,r=l.isNumber,
k=l.isObject,t=l.merge,e;(function(a){function e(a,b){var c=[];if(r(a)&&r(b)&&a<=b)for(;a<=b;a++)c.push(a);return c}a.recursive=d.prototype.utils.recursive;a.calculateLevelSizes=function(a,b){b=k(b)?b:{};var c=0,d;if(k(a)){var g=t({},a);a=r(b.from)?b.from:0;var l=r(b.to)?b.to:0;var v=e(a,l);a=Object.keys(g).filter(function(a){return-1===v.indexOf(+a)});var w=d=r(b.diffRadius)?b.diffRadius:0;v.forEach(function(a){a=g[a];var b=a.levelSize.unit,e=a.levelSize.value;"weight"===b?c+=e:"percentage"===b?
(a.levelSize={unit:"pixels",value:e/100*w},d-=a.levelSize.value):"pixels"===b&&(d-=e)});v.forEach(function(a){var b=g[a];"weight"===b.levelSize.unit&&(b=b.levelSize.value,g[a].levelSize={unit:"pixels",value:b/c*d})});a.forEach(function(a){g[a].levelSize={value:0,unit:"pixels"}})}return g};a.getLevelFromAndTo=function(a){var b=a.level;return{from:0<b?b:1,to:b+a.height}};a.range=e})(e||(e={}));return e});w(a,"Series/Sunburst/SunburstSeries.js",[a["Series/CenteredUtilities.js"],a["Core/Globals.js"],
a["Core/Series/SeriesRegistry.js"],a["Series/Sunburst/SunburstPoint.js"],a["Series/Sunburst/SunburstUtilities.js"],a["Series/TreeUtilities.js"],a["Core/Utilities.js"]],function(a,l,d,r,k,t,e){function g(a,b){var c=b.mapIdToNode[a.parent],d=b.series,e=d.chart,g=d.points[a.i];c=u(a,{colors:d.options.colors||e&&e.options.colors,colorIndex:d.colorIndex,index:b.index,mapOptionsToLevel:b.mapOptionsToLevel,parentColor:c&&c.color,parentColorIndex:c&&c.colorIndex,series:b.series,siblings:b.siblings});a.color=
c.color;a.colorIndex=c.colorIndex;g&&(g.color=a.color,g.colorIndex=a.colorIndex,a.sliced=a.id!==b.idRoot?g.sliced:!1);return a}var v=this&&this.__extends||function(){var a=function(b,c){a=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(a,b){a.__proto__=b}||function(a,b){for(var c in b)b.hasOwnProperty(c)&&(a[c]=b[c])};return a(b,c)};return function(b,c){function d(){this.constructor=b}a(b,c);b.prototype=null===c?Object.create(c):(d.prototype=c.prototype,new d)}}(),c=a.getCenter,b=
a.getStartAndEndRadians;a=l.noop;var w=d.series,y=d.seriesTypes;l=y.column;var m=y.treemap,u=t.getColor,x=t.getLevelOptions,C=t.setTreeValues,p=t.updateRootId,K=e.error,L=e.extend,D=e.isNumber,H=e.isObject,G=e.isString,I=e.merge,J=e.splat,N=180/Math.PI;t=function(a){function d(){var b=null!==a&&a.apply(this,arguments)||this;b.center=void 0;b.data=void 0;b.mapOptionsToLevel=void 0;b.nodeMap=void 0;b.options=void 0;b.points=void 0;b.shapeRoot=void 0;b.startAndEndRadians=void 0;b.tree=void 0;return b}
v(d,a);d.prototype.alignDataLabel=function(b,c,d){if(!d.textPath||!d.textPath.enabled)return a.prototype.alignDataLabel.apply(this,arguments)};d.prototype.animate=function(a){var b=this.chart,c=[b.plotWidth/2,b.plotHeight/2],d=b.plotLeft,e=b.plotTop;b=this.group;a?(a={translateX:c[0]+d,translateY:c[1]+e,scaleX:.001,scaleY:.001,rotation:10,opacity:.01},b.attr(a)):(a={translateX:d,translateY:e,scaleX:1,scaleY:1,rotation:0,opacity:1},b.animate(a,this.options.animation))};d.prototype.drawPoints=function(){var a=
this,b=a.mapOptionsToLevel,c=a.shapeRoot,d=a.group,e=a.hasRendered,g=a.rootNode,f=a.idPreviousRoot,l=a.nodeMap,k=l[f],m=k&&k.shapeArgs;k=a.points;var p=a.startAndEndRadians,r=a.chart,t=r&&r.options&&r.options.chart||{},v="boolean"===typeof t.animation?t.animation:!0,u=a.center[3]/2,y=a.chart.renderer,x=!1,A=!1;if(t=!!(v&&e&&g!==f&&a.dataLabelsGroup)){a.dataLabelsGroup.attr({opacity:0});var C=function(){x=!0;a.dataLabelsGroup&&a.dataLabelsGroup.animate({opacity:1,visibility:"visible"})}}k.forEach(function(h){var k=
h.node,n=b[k.level];var t=h.shapeExisting||{};var q=k.shapeArgs||{},w=!(!k.visible||!k.shapeArgs);if(e&&v){var B={};var E={end:q.end,start:q.start,innerR:q.innerR,r:q.r,x:q.x,y:q.y};w?!h.graphic&&m&&(B=g===h.id?{start:p.start,end:p.end}:m.end<=q.start?{start:p.end,end:p.end}:{start:p.start,end:p.start},B.innerR=B.r=u):h.graphic&&(f===h.id?E={innerR:u,r:u}:c&&(E=c.end<=t.start?{innerR:u,r:u,start:p.end,end:p.end}:{innerR:u,r:u,start:p.start,end:p.start}));t=B}else E=q,t={};B=[q.plotX,q.plotY];if(!h.node.isLeaf)if(g===
h.id){var z=l[g];z=z.parent}else z=h.id;L(h,{shapeExisting:q,tooltipPos:B,drillId:z,name:""+(h.name||h.id||h.index),plotX:q.plotX,plotY:q.plotY,value:k.val,isInside:w,isNull:!w});z=h.options;k=H(q)?q:{};z=H(z)?z.dataLabels:{};n=J(H(n)?n.dataLabels:{})[0];n=I({style:{}},n,z);z=n.rotationMode;if(!D(n.rotation)){if("auto"===z||"circular"===z)if(1>h.innerArcLength&&h.outerArcLength>k.radius){var x=0;h.dataLabelPath&&"circular"===z&&(n.textPath={enabled:!0})}else 1<h.innerArcLength&&h.outerArcLength>1.5*
k.radius?"circular"===z?n.textPath={enabled:!0,attributes:{dy:5}}:z="parallel":(h.dataLabel&&h.dataLabel.textPathWrapper&&"circular"===z&&(n.textPath={enabled:!1}),z="perpendicular");"auto"!==z&&"circular"!==z&&(x=k.end-(k.end-k.start)/2);n.style.width="parallel"===z?Math.min(2.5*k.radius,(h.outerArcLength+h.innerArcLength)/2):k.radius;"perpendicular"===z&&h.series.chart.renderer.fontMetrics(n.style.fontSize).h>h.outerArcLength&&(n.style.width=1);n.style.width=Math.max(n.style.width-2*(n.padding||
0),1);x=x*N%180;"parallel"===z&&(x-=90);90<x?x-=180:-90>x&&(x+=180);n.rotation=x}n.textPath&&(0===h.shapeExisting.innerR&&n.textPath.enabled?(n.rotation=0,n.textPath.enabled=!1,n.style.width=Math.max(2*h.shapeExisting.r-2*(n.padding||0),1)):h.dlOptions&&h.dlOptions.textPath&&!h.dlOptions.textPath.enabled&&"circular"===z&&(n.textPath.enabled=!0),n.textPath.enabled&&(n.rotation=0,n.style.width=Math.max((h.outerArcLength+h.innerArcLength)/2-2*(n.padding||0),1)));0===n.rotation&&(n.rotation=.001);h.dlOptions=
n;if(!A&&w){A=!0;var S=C}h.draw({animatableAttribs:E,attribs:L(t,!r.styledMode&&a.pointAttribs(h,h.selected&&"select")),onComplete:S,group:d,renderer:y,shapeType:"arc",shapeArgs:q})});t&&A?(a.hasRendered=!1,a.options.dataLabels.defer=!0,w.prototype.drawDataLabels.call(a),a.hasRendered=!0,x&&C()):w.prototype.drawDataLabels.call(a)};d.prototype.layoutAlgorithm=function(a,b,c){var d=a.start,e=a.end-d,h=a.val,f=a.x,g=a.y,k=c&&H(c.levelSize)&&D(c.levelSize.value)?c.levelSize.value:0,l=a.r,m=l+k,p=c&&D(c.slicedOffset)?
c.slicedOffset:0;return(b||[]).reduce(function(a,b){var c=1/h*b.val*e,n=d+c/2,q=f+Math.cos(n)*p;n=g+Math.sin(n)*p;b={x:b.sliced?q:f,y:b.sliced?n:g,innerR:l,r:m,radius:k,start:d,end:d+c};a.push(b);d=b.end;return a},[])};d.prototype.setShapeArgs=function(a,b,c){var d=[],e=c[a.level+1];a=a.children.filter(function(a){return a.visible});d=this.layoutAlgorithm(b,a,e);a.forEach(function(a,b){b=d[b];var e=b.start+(b.end-b.start)/2,f=b.innerR+(b.r-b.innerR)/2,h=b.end-b.start;f=0===b.innerR&&6.28<h?{x:b.x,
y:b.y}:{x:b.x+Math.cos(e)*f,y:b.y+Math.sin(e)*f};var g=a.val?a.childrenTotal>a.val?a.childrenTotal:a.val:a.childrenTotal;this.points[a.i]&&(this.points[a.i].innerArcLength=h*b.innerR,this.points[a.i].outerArcLength=h*b.r);a.shapeArgs=I(b,{plotX:f.x,plotY:f.y+4*Math.abs(Math.cos(e))});a.values=I(b,{val:g});a.children.length&&this.setShapeArgs(a,a.values,c)},this)};d.prototype.translate=function(){var a=this,d=a.options,e=a.center=c.call(a),l=a.startAndEndRadians=b(d.startAngle,d.endAngle),m=e[3]/2,
t=e[2]/2-m,f=p(a),n=a.nodeMap,q=n&&n[f],r={};a.shapeRoot=q&&q.shapeArgs;w.prototype.translate.call(a);var v=a.tree=a.getTree();n=a.nodeMap;q=n[f];var u=G(q.parent)?q.parent:"";u=n[u];var y=k.getLevelFromAndTo(q);var A=y.from,D=y.to;y=x({from:A,levels:a.options.levels,to:D,defaults:{colorByPoint:d.colorByPoint,dataLabels:d.dataLabels,levelIsConstant:d.levelIsConstant,levelSize:d.levelSize,slicedOffset:d.slicedOffset}});y=k.calculateLevelSizes(y,{diffRadius:t,from:A,to:D});C(v,{before:g,idRoot:f,levelIsConstant:d.levelIsConstant,
mapOptionsToLevel:y,mapIdToNode:n,points:a.points,series:a});d=n[""].shapeArgs={end:l.end,r:m,start:l.start,val:q.val,x:e[0],y:e[1]};this.setShapeArgs(u,d,y);a.mapOptionsToLevel=y;a.data.forEach(function(b){r[b.id]&&K(31,!1,a.chart);r[b.id]=!0});r={}};d.defaultOptions=I(m.defaultOptions,{center:["50%","50%"],colorByPoint:!1,opacity:1,dataLabels:{allowOverlap:!0,defer:!0,rotationMode:"auto",style:{textOverflow:"ellipsis"}},rootId:void 0,levelIsConstant:!0,levelSize:{value:1,unit:"weight"},slicedOffset:10});
return d}(m);L(t.prototype,{drawDataLabels:a,pointAttribs:l.prototype.pointAttribs,pointClass:r,utils:k});d.registerSeriesType("sunburst",t);"";return t});w(a,"masters/modules/sunburst.src.js",[],function(){})});
//# sourceMappingURL=sunburst.js.map