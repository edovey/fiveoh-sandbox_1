//
//  CategoryView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface CategoryListView : UIViewController <UITableViewDataSource, UITableViewDelegate>

@property (nonatomic, retain) NSArray *categoryArray;
@property (retain, nonatomic) IBOutlet UITableView *dataTableView;
@property (nonatomic, retain) NSString *parentId;
@property (nonatomic, retain) NSString *parentName;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName;

@end
