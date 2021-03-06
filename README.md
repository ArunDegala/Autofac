Autofac
=======

A GitHub clone of the excellent Autofac library on GoogleCode

_Every_ possible (read: reasonable) effort will be made to keep this repo up-to-date with the [Hg repo out on code.google.com](http://code.google.com/p/autofac/).

Please note that only the "[default](http://code.google.com/p/autofac/source/list?name=default)" branch from the original source will make it into this repo; I don't have the bandwidth to track and merge every change from every branch over there.

I will be using [fast-export](http://hivelogic.com/articles/converting-from-mercurial-to-git/) to convert the source repo's commits to git commits. I'll be pushing the resulting source master branch into the 'hg-master' branch in this repo. That branch will then be merged into master, dev, and any feature branches. Except for that, I'll be following [this branching model](http://nvie.com/posts/a-successful-git-branching-model/). I'm prepared to accept pull requests for any branch I have pushed up other than hg-master.

Any changes that make it into master will be sent to the original authors @ GoogleCode as patches. Maybe our contributions can make it into the official build!

For those that live on the bleeding edge, I'll be pushing a Community.Autofac.* package series up to NuGet using this repo's code.

Please see [the original project](http://code.google.com/p/autofac/) for more info.