﻿GlobalController=
["$scope","$http","$location",'$window',"Channel" 
($scope,$http,$location,$window,Channel) ->
  $http.get("/admin/getuser").success (data) ->
    if data.Email
      data.Gravatar='http://www.gravatar.com/avatar/' + md5(data.Email) 
    else
      data.Gravatar='/Content/img/avatar.png'
    $scope.User=data

  Channel.query 
    $orderby:'OrderId' 
    $filter:'IsDeleted eq false'
    $expand:'Groups'
    $select:'Name,Url,Groups/Name,Groups/Url,Groups/IsDeleted,Groups/OrderId'
  ,(data)->
    $scope.Channels=data.value

  $scope.search = ->
    $window.location.href="/#!/search/#{$scope.key}"
]