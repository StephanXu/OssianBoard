<template>
  <div id="app">
    <router-view/>
  </div>
</template>
<script>
export default {
  beforeCreate() {
    this.$vuetify.theme.dark = true;
  }
}
</script>
<style lang="scss">

</style>
