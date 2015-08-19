(function(window, undefined) {
	var S = window.Searchanise || {};

	S.host = (S.host || 'www.searchanise.com').split('http://').join('').split('https://').join('');

	S.userOptions = S.options || {};
	S.AutoCmpParams = S.AutoCmpParams || {}
	S.userOptions.api_key = S.AutoCmpParams.api_key = S.api_key || S.ApiKey;
	S.userOptions.SearchInput = S.SearchInput;

	S.version = '1.0';
	S.protocol = ('https:' == document.location.protocol ? 'https://' : 'http://');

	S.paths = {};
	S.paths.jq      = S.jq      || S.protocol + 'ajax.googleapis.com/ajax/libs/jquery/1.6.3/jquery.min.js';
	S.paths.widgets = S.widgets || S.protocol + ((S.protocol == 'https://')? 's3.amazonaws.com/' : '') + 'static.searchanise.com/widgets.76895.min.js';

	S.paths.tpl     = S.tpl     || S.protocol + ((S.protocol == 'https://')? 's3.amazonaws.com/' : '') + 'static.searchanise.com/templates.[API_KEY].js'.split('[API_KEY]').join(S.userOptions.api_key);
	S.paths.style   = S.style   || S.protocol + ((S.protocol == 'https://')? 's3.amazonaws.com/' : '') + 'static.searchanise.com/styles.[API_KEY].css'.split('[API_KEY]').join(S.userOptions.api_key);
	S.paths.preload = S.preload || S.protocol + ((S.protocol == 'https://')? 's3.amazonaws.com/' : '') + 'static.searchanise.com/preload_data.[API_KEY].js'.split('[API_KEY]').join(S.userOptions.api_key);

	S.error = function(err) {

	};

	if (!S.userOptions.api_key) {
		S.error('Searchanise: Api key wasn\'t found');
		return;
	}

	S.loadScript = function(href, callback) {
		var script = document.createElement('script');
		script.charset = 'utf-8';
		script.src = href;
		
		script.onload = script.onreadystatechange = function() {
			if (!script.readyState || /loaded|complete/.test( script.readyState )) {
				script.onload = script.onreadystatechange = null;
				script = undefined;
				if (typeof(callback) == 'function') {
					callback();
				}
			}
		};
		document.getElementsByTagName('head')[0].appendChild(script);
	};

	S.loadCss = function(href) {
		var style = document.createElement('link');
		style.rel = 'stylesheet';
		style.href = href;
		style.className = 'snize_widget_css';
		document.getElementsByTagName('head')[0].appendChild(style);
	};

	S.loader = {
		callback: null,
 
		loadedCount: 0,
		isLoaded: 4, //jQuery + Widgets + templates + DOMcontent

		loaded: function() {
			S.loader.loadedCount ++;
			if (this.loadedCount == this.isLoaded) {
				S.loader.callback();
			}
		},
 
		jqLoaded: function() {
			S.loadScript(S.paths.widgets, function() {
				S.loader.loaded();
			});

			if (S.$(S.SearchInput).length || S.$(S.options.ResultsDiv).length) {
				S.loader.loaded();
			} else {
				var interval = setInterval(function() {
					if (S.$(S.SearchInput).length || S.$(S.options.ResultsDiv).length) {
						S.loader.loaded();
						clearInterval(interval);
					};
				}, 100);
			}

			S.loader.loaded();
		},
		init: function(callback) {

			S.loader.callback = callback;

			/* Load templates */
			S.loadScript(S.paths.tpl, function() {
				if (window.Searchaise_templates) {
					S.loadOptions = {};
					for (var i in Searchaise_templates) {
						S.loadOptions[i] = Searchaise_templates[i];
					}

					try {
						delete window.Searchaise_templates;
					} catch(e) {
						window.Searchaise_templates = undefined;
					}
				}
				S.loader.loaded();
			});

			S.loadCss(S.paths.style);

			if (!S.forcejQuery && window.jQuery) {
				S.$ = window.jQuery;
				S.loader.jqLoaded();

			} else if(!S.forcejQuery && window.SNIZE && window.SNIZE.$) {
				S.$ = window.SNIZE.$;
				S.loader.jqLoaded();

			} else {
				S.loadScript(S.paths.jq, function() {
					jQuery.noConflict();
					S.$ = window.jQuery;
					S.loader.jqLoaded();
				});
			}

			S.loadScript(S.paths.preload);
		}
	};

	S.loader.init(function() {
		if (!S.Loaded) {
		//return;
		}

		if (S.AutoCmpParams) {
			S.SetParams(S.AutoCmpParams);
		}

		S.Init();
		S.SetPaths(S.paths);
		S.SetOptions(S.loadOptions);
		S.SetOptions(S.userOptions);
		S.Start();
		S.Loaded = true;
	});

	/* Active export to window */
	window.Searchanise = Searchanise;
}(window));