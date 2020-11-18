<template>
  <div id="app">
    <router-view/>
  </div>
</template>
<script>
export default {
  computed: {
    scrollbarTheme() {
      return this.$vuetify.theme.dark ? 'dark' : 'light';
    }
  },
  beforeCreate() {
    this.$vuetify.theme.dark = true;
  },
  mounted() {
    this.$store.commit("view/SET_CLIENT_HEIGHT", document.documentElement.clientHeight);
    this.$store.commit("view/SET_CLIENT_WIDTH", document.documentElement.clientWidth);
    const _this = this;
    window.onresize = function temp() {
      _this.$store.commit(
          "view/SET_CLIENT_HEIGHT",
          document.documentElement.clientHeight
      );
      _this.$store.commit(
          "view/SET_CLIENT_WIDTH",
          document.documentElement.clientWidth
      );
    };
  }
}
</script>
<style>

.light::-webkit-scrollbar  {
  width: 15px;
  height: 1px;
}

.light::-webkit-scrollbar-thumb  {
  -webkit-box-shadow: inset 0 0 5px rgba(255, 255, 255, 0.2);
  background: #b0b0b0;
}

.light::-webkit-scrollbar-track  {
  -webkit-box-shadow: inset 0 0 5px rgba(255, 255, 255, 0.2);
  background: #f6f6f6;
}

.light::-webkit-scrollbar-thumb:hover  {
  background: #a0a0a0;
}

.dark::-webkit-scrollbar  {
  width: 15px;
  height: 1px;
}

.dark::-webkit-scrollbar-thumb  {
  -webkit-box-shadow: inset 0 0 5px rgba(0, 0, 0, 0.2);
  background: #535353;
}

.dark::-webkit-scrollbar-track {
  -webkit-box-shadow: inset 0 0 5px rgba(0, 0, 0, 0.2);
  background: #181818;
}

.dark::-webkit-scrollbar-thumb:hover {
  background: #a0a0a0;
}

</style>
