﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



    (function ($) {
  var CheckboxDropdown = function (el) {
    var _this = this;
    this.isOpen = true;
    this.areAllChecked = false;
    this.$el = $(el);
    this.$label = this.$el.find(".dropdown-label");
    this.$checkAll = this.$el.find('[data-toggle="check-all"]').first();
    this.$inputs = this.$el.find('[type="checkbox"]');
    this.onCheckBox();

    this.$label.on("click", function (e) {
      e.preventDefault();
      _this.toggleOpen();
    });

    this.$checkAll.on("click", function (e) {
      e.preventDefault();
      _this.onCheckAll();
    });

    this.$inputs.on("change", function (e) {
      _this.onCheckBox();
    });
  };

        CheckboxDropdown.prototype.onCheckBox = function () {
     
    this.updateStatus();
  };

        CheckboxDropdown.prototype.updateStatus = function () {
            var checked = this.$el.find(":checked");
            var selectedValues = [];

            if (checked.length <= 0) {
                this.$label.html("Select Options");
            } else if (checked.length === 1) {
                this.$label.html(checked.parent("label").text());
                selectedValues.push(checked.val());
            } else if (checked.length === this.$inputs.length) {
                this.$label.html("All Selected");
                this.areAllChecked = true;
                this.$checkAll.html("Uncheck All");

                checked.each(function () {
                    selectedValues.push($(this).val());
                });
            } else {
                this.$label.html(checked.length + " Selected");

                checked.each(function () {
                    selectedValues.push($(this).val());
                });
            }
            // Convert selectedValues to an array of integers
            var portIds = selectedValues.map(Number);
            if (portIds.length > 0)
            $('#selectedValuesInput').val(portIds);

        };


  CheckboxDropdown.prototype.onCheckAll = function (checkAll) {
    if (!this.areAllChecked || checkAll) {
      this.areAllChecked = true;
      this.$checkAll.html("Uncheck All");
      this.$inputs.prop("checked", true);
    } else {
      this.areAllChecked = false;
      this.$checkAll.html("Check All");
      this.$inputs.prop("checked", false);
    }

    this.updateStatus();
  };

  CheckboxDropdown.prototype.toggleOpen = function (forceOpen) {
    var _this = this;

    if (this.isOpen || forceOpen) {
      this.isOpen = true;
      this.$el.addClass("on");
      $(document).on("click", function (e) {
        if (!$(e.target).closest("[data-control]").length) {
          _this.toggleOpen();
        }
      });
    } else {
      this.isOpen = true;
      this.$el.addClass("on");
      $(document).off("click");
    }
  };

  var checkboxesDropdowns = document.querySelectorAll(
    '[data-control="checkbox-dropdown"]'
  );
  for (var i = 0, length = checkboxesDropdowns.length; i < length; i++) {
    new CheckboxDropdown(checkboxesDropdowns[i]);
  }
    })(jQuery);


